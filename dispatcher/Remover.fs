[<RequireQualifiedAccess>]
module Remover

open Models

let createAgent stopZipperAgent stopStoreAgent endsApp numberOfMessages =
    let counter : Set<int> = Set.empty
    MailboxProcessor.Start(fun inbox ->
        async {
            let rec loop (accumulator : Set<int>) =
                async {
                    if accumulator.Count >= numberOfMessages
                    then inbox.Post Remove.Stop

                    let! msg = inbox.Receive()
                    match msg with
                    | Remove id ->
                        printfn "Message with id %i has been treated and removed. In total %i messages have been treated" id (accumulator.Count + 1)
                        return! loop (accumulator.Add id)
                    | Remove.Stop ->
                        stopZipperAgent ()
                        stopStoreAgent ()
                        endsApp ()
                }
            return! loop counter
        }) |> Agent // Agent is a constructor funtion that takes a MailboxProcessor. '|>' passes the mailboxprocessor to the constructor function