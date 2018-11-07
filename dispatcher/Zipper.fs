[<RequireQualifiedAccess>]
module Zipper

open System
open Models
open System.Text

let createAgent postMessage =
    MailboxProcessor.Start(fun inbox ->
        async {
            let rec loop () =
                async {
                    let! msg = inbox.Receive()
                    match msg with
                    | Hydratated message ->
                        // not really zipping but... is a POC I mean!
                        let zippedBody = Encoding.UTF8.GetBytes message.Body |> Convert.ToBase64String
                        { ZippedMessage.Id = message.Id; ZippedMessage.Body = zippedBody }
                        |> Zipped
                        |> postMessage

                        return! loop()
                    | Hydratated.Stop -> ()
                }
            return! loop()
        }) |> Agent // Agent is a constructor funtion that takes a MailboxProcessor. '|>' passes the mailboxprocessor to the constructor function


