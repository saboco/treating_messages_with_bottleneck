[<RequireQualifiedAccess>]
module Zipper

open System
open Models
open System.Text
open System.Diagnostics

let createAgent postMessage =
    MailboxProcessor.Start(fun inbox ->
        async {
            while true do
                let! (Hydratated message) = inbox.Receive()
                Debug.WriteLine(sprintf "Zipper: %A" message)
                // not really zipping but... is a POC I mean!
                let zippedBody = Encoding.UTF8.GetBytes message.Body |> Convert.ToBase64String
                { ZippedMessage.Id = message.Id; ZippedMessage.Body = zippedBody }
                |> Zipped
                |> postMessage

        }) |> Agent // Agent is a constructor funtion that takes a MailboxProcessor. '|>' passes the mailboxprocessor to the constructor function


