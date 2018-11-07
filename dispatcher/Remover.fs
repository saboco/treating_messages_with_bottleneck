[<RequireQualifiedAccess>]
module Remover

open Models
open System.Net.Http
open Newtonsoft.Json

let createAgent (url : System.Uri) stopZipperAgent stopStoreAgent endsApp numberOfMessages batchSize =
    let counter : Set<int> = Set.empty

    let treatMessages messages =
        async {
            let url = Url.createUri url "/treated"
            use client = new HttpClient()
            let content = new StringContent(JsonConvert.SerializeObject messages)
            let! res = client.PostAsync(url, content) |> Async.AwaitTask
            return! res.Content.ReadAsStringAsync() |> Async.AwaitTask
        }

    MailboxProcessor.Start(fun inbox ->
        async {
            let rec loop (accumulator : Set<int>) (toRemove : int list) =
                async {
                    if accumulator.Count >= numberOfMessages
                    then inbox.Post Remove.Stop

                    let! msg = inbox.Receive()
                    match msg with
                    | Remove id when toRemove.Length = batchSize ->
                        let toRemove = (List.Cons(id,toRemove))
                        let! body = treatMessages toRemove

                        printfn "%s" body

                        return! loop (accumulator.Add id) []
                    | Remove id ->
                        return! loop (accumulator.Add id) (List.Cons(id,toRemove))
                    | Remove.Stop ->
                        if toRemove.Length > 0 then
                            let! _ = treatMessages toRemove
                            ()

                        stopZipperAgent ()
                        stopStoreAgent ()
                        endsApp ()
                }
            return! loop counter []
        }) |> Agent // Agent is a constructor funtion that takes a MailboxProcessor. '|>' passes the mailboxprocessor to the constructor function