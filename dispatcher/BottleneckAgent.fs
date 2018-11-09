[<RequireQualifiedAccess>]
module Bottleneck

open Models
open System.Net.Http
open Newtonsoft.Json

let createAgent (url : System.Uri) postMessage () =
    MailboxProcessor.Start(fun inbox ->
        async {
            let rec loop () =
                async {
                    let! msgO = inbox.TryReceive(5000)
                    match msgO with
                    | Some msg ->
                        match msg with
                        | Fetched message ->
                            use client = new HttpClient()
                            let content = new StringContent(JsonConvert.SerializeObject message)
                            let! res = client.PostAsync(url, content) |> Async.AwaitTask
                            let! body = res.Content.ReadAsStringAsync() |> Async.AwaitTask

                            // post message to next step
                            JsonConvert.DeserializeObject<Message list> body
                            |> List.map (fun message -> message |> Hydratated |> postMessage)
                            |> ignore

                            return! loop()

                        | Fetched.Stop -> ()
                    | None -> 
                        printfn "Bottleneck agent is starving...."
                        return! loop ()
                }
            return! loop ()

        }) |> Agent // Agent is a constructor funtion that takes a MailboxProcessor. '|>' passes the mailboxprocessor to the constructor function
