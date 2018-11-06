// Learn more about F# at http://fsharp.org

open System
open Models
open Microsoft.AspNetCore.Hosting

open BenchmarkDotNet.Attributes
open System.Threading.Tasks
open BenchmarkDotNet.Running


let runDispatcher url messagesToTreat numberOfAgents =
    let (Agent removerAgent) = Remover.createAgent ()
    let (Agent storeAgent) = Store.createAgent removerAgent.Post
    let (Agent zipperAgent) = Zipper.createAgent storeAgent.Post
    let createBottleneckAgent =  Bottleneck.createAgent url zipperAgent.Post
    let getBottleneckAgent = AgentsPool.createAgentSelector (numberOfAgents, createBottleneckAgent)

    let fetchMessages = RawMessages.fetchMessagesAgent removerAgent.Post getBottleneckAgent
    let n = messagesToTreat
    let task =fetchMessages n |> Async.StartAsTask
    task.Wait()

let runBottleneck () = bottleneck.App.host.RunAsync()
let runApp numberOfMessages numberOfAgents =
    runDispatcher (Uri "http://localhost:5000/api/cassandra") numberOfMessages numberOfAgents


type BottleneckBenchMark () =

    [<Params(1,8,32,64,128,512,1024)>]
    //[<Params(512)>]
    member val public NumberOfAgents = 0 with get,set

    [<Params(3000)>]
    member val public MessagesToTreat = 0 with get, set

    [<GlobalSetup>]
    member __.SetupData () =
        let _ = runBottleneck () |> Async.AwaitTask |> Async.StartAsTask
        Task.Delay(2000) |> Async.AwaitTask |> Async.RunSynchronously // waiting so the server starts


    [<Benchmark>]
    member this.App () =
        runApp this.MessagesToTreat this.NumberOfAgents

        let rec waitUntilAllMessagesHasBeenTreated ()  =
            let count = Remover.getCount ()
            if count >= this.MessagesToTreat
            then count
            else waitUntilAllMessagesHasBeenTreated ()
        waitUntilAllMessagesHasBeenTreated()

[<EntryPoint>]
let main argv =

    let switch =
        BenchmarkSwitcher [|
            typeof<BottleneckBenchMark>
        |]

    switch.Run argv |> ignore
    0
