[<RequireQualifiedAccess>]
module Store

open System.IO
open Models
open Newtonsoft.Json
open System.Diagnostics


let createAgent postMessage =
    let storePath =
        let here = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
        Path.Combine(here, "messages")
    let getFullPath file =
        Path.Combine(storePath, file)
    let ensureStore () =
        if not (Directory.Exists storePath) then
            Directory.CreateDirectory storePath |> ignore
    ensureStore ()
    MailboxProcessor.Start(fun inbox ->
        async {
            while true do
                let! (Zipped zippedMessage) = inbox.Receive()
                Debug.WriteLine(sprintf "Store: %A" zippedMessage)
                let path = (sprintf "message%i" zippedMessage.Id) |> getFullPath
                do! File.AppendAllTextAsync(path, JsonConvert.SerializeObject zippedMessage) |> Async.AwaitTask
                
                zippedMessage.Id
                |> Remove
                |> postMessage

        }) |> Agent // Agent is a constructor funtion that takes a MailboxProcessor. '|>' passes the mailboxprocessor to the constructor function
