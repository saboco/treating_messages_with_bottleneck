// Learn more about F# at http://fsharp.org

open System
open Models
open System

[<EntryPoint>]
let main _ =
    let rec getNumber () = 
        let n = Console.ReadLine ()
        match Int32.TryParse n with
        | true, i -> i
        | _ -> 
            printfn "wrong input"
            getNumber ()

    let messagesToTreat = 
        printfn "How many messages?"
        getNumber ()

    let numberOfAgents =
        printfn "How many agents?"        
        getNumber ()



    let removeMessage = (fun (Remove id) -> printfn "Message with id %i has been treated and removed" id)
    let (Agent storeAgent) = Store.createAgent removeMessage
    let (Agent zipperAgent) = Zipper.createAgent storeAgent.Post
    let createBottleneckAgent =  Bottleneck.createAgent zipperAgent.Post
    let getBottleneckAgent = AgentsPool.createAgentSelector (numberOfAgents, createBottleneckAgent)

    let fetchMessages = RawMessages.fetchMessagesAgent getBottleneckAgent
    let n = messagesToTreat
    printfn "Fetching %i messages" n
    let a = fetchMessages n |> Async.RunSynchronously
    printfn "%i messages fetched" n
    Console.ReadLine () |> ignore
    0 // return an integer exit code
