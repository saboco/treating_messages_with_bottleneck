open System
open Models

open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open System.Net.Http

let clearMessagesStore () =
    async {
        use client = new HttpClient()
        let! res =client.DeleteAsync(Uri("http://localhost:5000/api/messages")) |> Async.AwaitTask
        let! body= res.Content.ReadAsStringAsync() |> Async.AwaitTask
        printfn "%s" body
        return ()
    }
    

let setupDispatcher bottleneckUrl messagesUrl messagesToTreat numberOfBottleneckAgents batchSize numberOfFetchAgents endsApp =
    let dummyPostMsg () : unit = failwith "unfixed reference"

    let stopStoreAgent = ref dummyPostMsg
    let stopZipperAgent = ref dummyPostMsg

    let runStopStoreAgent () = !stopStoreAgent ()
    let runStopZipperAgent () = !stopZipperAgent ()

    let (Agent removerAgent) = Remover.createAgent messagesUrl runStopStoreAgent runStopZipperAgent endsApp messagesToTreat batchSize
    let (Agent storeAgent) = Store.createAgent removerAgent.Post
    let (Agent zipperAgent) = Zipper.createAgent storeAgent.Post
    let createBottleneckAgent =  Bottleneck.createAgent bottleneckUrl zipperAgent.Post
    let getBottleneckAgent,stopAgents = AgentsPool.createAgentSelector (numberOfBottleneckAgents, createBottleneckAgent)

    let postStopStoreAgent () = storeAgent.Post Zipped.Stop
    let postStopZipperAgent () = zipperAgent.Post Hydratated.Stop

    stopStoreAgent := postStopStoreAgent
    stopZipperAgent := postStopZipperAgent

    let fetchMessages = RawMessages.fetchMessagesAgent messagesUrl removerAgent.Post getBottleneckAgent
    stopAgents, fetchMessages messagesToTreat batchSize numberOfFetchAgents

let testApp () =
    let mutable ends = false
    let endsApp () = ends <- true
    let bottleneckUrl = Uri "http://localhost:5000/api/bottleneck"
    let messagesUrl = Uri "http://localhost:5000/api/messages"
    let (stopagents, fetchMessages) =
        setupDispatcher bottleneckUrl messagesUrl 10000 8 1000 1 endsApp
    fetchMessages |> List.map Async.RunSynchronously |> ignore
    let rec loop () =
        if not ends
        then loop ()

    loop ()
    stopagents Fetched.Stop |> Async.RunSynchronously

[<RankColumn>]
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
        let innerFn () :  List<Async<unit>> = failwith "dummyApp unfixed"
        innerFn

    let dummyStopAgents =
        let innerFn () : AgentsPool.StopAgentFunc<Fetched> = failwith "dummyStopAgents unfixed"
        innerFn

    let app = ref dummyApp
    let stopAgents = ref dummyStopAgents

    let runApp () = !app () |> List.map Async.RunSynchronously |> ignore
    let runStopAgents () = !stopAgents () Fetched.Stop |> Async.RunSynchronously

    [<Params(128,512,1024)>]
    member val public NumberOfBottleneckAgents = 0 with get,set

    [<Params(1,8,128)>]
    member val public NumberOfFetchAgents = 0 with get,set

    [<Params(1000)>]
    member val public BatchSize = 0 with get,set

    [<Params(10000)>]
    member val public MessagesToTreat = 0 with get, set

    [<IterationSetup>]
    member this.Setup () =
        printfn "********************************************* IterationSetup"
        let bottleneckUrl = Uri "http://localhost:5000/api/bottleneck"
        let messagesUrl = Uri "http://localhost:5000/api/messages"
        let (a, b) = setupDispatcher bottleneckUrl messagesUrl this.MessagesToTreat this.NumberOfBottleneckAgents this.BatchSize this.NumberOfFetchAgents endsApp
        app := (fun () -> b)
        stopAgents := (fun () -> a)
        ends <- false

    [<IterationCleanup>]
    member __.Clean () =
        printfn "********************************************* IterationCleanApp"
        runStopAgents ()
        clearMessagesStore () |> Async.RunSynchronously

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
