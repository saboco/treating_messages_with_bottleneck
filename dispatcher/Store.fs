﻿[<RequireQualifiedAccess>]
module Store

open System.IO
open Models
open Newtonsoft.Json

let createAgent postMessage writeOnDisk =
    let storePath =
        let here = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
        let storeId = System.Guid.NewGuid()
        Path.Combine(here, sprintf "messages/%O" storeId)
    let getFullPath file =
        Path.Combine(storePath, file)
    let ensureStore () =
        if not (Directory.Exists storePath) then
            Directory.CreateDirectory storePath |> ignore
    ensureStore ()
    MailboxProcessor.Start(fun inbox ->
        async {
            let rec loop () =
                async {
                    let! msgO = inbox.TryReceive(5000)
                    match msgO with
                    | Some msg ->
                        match msg with
                        | Zipped zippedMessage ->
                            
                            if writeOnDisk then
                                let path = (sprintf "message%i" zippedMessage.Id) |> getFullPath
                                do! File.AppendAllTextAsync(path, JsonConvert.SerializeObject zippedMessage) |> Async.AwaitTask

                            zippedMessage.Id
                            |> Remove
                            |> postMessage

                            return! loop()
                        | Zipped.Stop -> ()
                    | None ->
                        printfn "Store is starving...."
                        return! loop ()
                }
            return! loop ()
        }) |> Agent // Agent is a constructor funtion that takes a MailboxProcessor. '|>' passes the mailboxprocessor to the constructor function
