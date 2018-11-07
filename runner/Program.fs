open System
open Models

open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running


let setupDispatcher url messagesToTreat numberOfAgents endsApp =
    let dummyPostMsg () : unit = failwith "unfixed reference"

    let stopStoreAgent = ref dummyPostMsg
    let stopZipperAgent = ref dummyPostMsg

    let runStopStoreAgent () = !stopStoreAgent ()
    let runStopZipperAgent () = !stopZipperAgent ()

    let (Agent removerAgent) = Remover.createAgent runStopStoreAgent runStopZipperAgent endsApp messagesToTreat
    let (Agent storeAgent) = Store.createAgent removerAgent.Post
    let (Agent zipperAgent) = Zipper.createAgent storeAgent.Post
    let createBottleneckAgent =  Bottleneck.createAgent url zipperAgent.Post
    let getBottleneckAgent,stopAgents = AgentsPool.createAgentSelector (numberOfAgents, createBottleneckAgent)

    let postStopStoreAgent () = storeAgent.Post Zipped.Stop
    let postStopZipperAgent () = zipperAgent.Post Hydratated.Stop

    stopStoreAgent := postStopStoreAgent
    stopZipperAgent := postStopZipperAgent

    let fetchMessages = RawMessages.fetchMessagesAgent removerAgent.Post getBottleneckAgent
    stopAgents, fetchMessages messagesToTreat

[<RPlotExporter; RankColumn>]
[<IterationCount(10)>]
type BottleneckBenchMark () =
    let mutable ends = false
    let oneloopMore () = not ends
    let rec loop () =
        if oneloopMore ()
        then
            printf "" // weird but without this the loop does not end
            loop ()

    let endsApp () = ends <- true

    let dummyApp =
        let innerFn () :  Async<unit> = failwith "dummyApp unfixed"
        innerFn

    let dummyStopAgents =
        let innerFn () : AgentsPool.StopAgentFunc<Fetched> = failwith "dummyStopAgents unfixed"
        innerFn

    let app = ref dummyApp
    let stopAgents = ref dummyStopAgents

    let runApp () = !app () |> Async.RunSynchronously
    let runStopAgents () = !stopAgents () Fetched.Stop |> Async.RunSynchronously


    [<Params(8,32,64,128,512,1024)>]
    member val public NumberOfAgents = 0 with get,set

    [<Params(10000)>]
    member val public MessagesToTreat = 0 with get, set

    [<IterationSetup>]
    member this.Setup () =
        printfn "********************************************* IterationSetup"
        let url =(Uri "http://localhost:5000/api/cassandra")
        let (a, b) = setupDispatcher url this.MessagesToTreat this.NumberOfAgents endsApp
        app := (fun () -> b)
        stopAgents := (fun () -> a)
        ends <- false

    [<IterationCleanup>]
    member __.Clean () =
        printfn "********************************************* IterationCleanApp"
        runStopAgents ()

    [<Benchmark>]
    member __.App () =
        printfn "********************************************* runningApp"
        runApp ()
        loop ()
        printfn "********************************************* endingApp"


[<EntryPoint>]
let main argv =

    let switch =
        BenchmarkSwitcher [|
            typeof<BottleneckBenchMark>
        |]

    switch.Run argv |> ignore

    0
