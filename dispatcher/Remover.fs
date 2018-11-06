[<RequireQualifiedAccess>]
module Remover

open Models

type Counter () =
    let mutable messageTreated : Set<int> = Set.empty

    member __.Add id =
        messageTreated <- messageTreated.Add id

    member __.Count () = messageTreated.Count

let private counter = Counter ()

let createAgent () =

    MailboxProcessor.Start(fun inbox ->
        async {
            while true do
                let! (Remove id) = inbox.Receive()
                counter.Add id
                printfn "Message with id %i has been treated and removed. In total %i messages have been treated" id (counter.Count ())
        }) |> Agent // Agent is a constructor funtion that takes a MailboxProcessor. '|>' passes the mailboxprocessor to the constructor function

let getCount () = counter.Count ()