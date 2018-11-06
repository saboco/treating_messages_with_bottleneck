[<RequireQualifiedAccess>]
module Bottleneck

open Models
open System.Net.Http
open Newtonsoft.Json
open System.Diagnostics

let createAgent postMessage () =
    MailboxProcessor.Start(fun inbox ->
        async {
            while true do
                let! (Fetched message)= inbox.Receive()
                Debug.WriteLine(sprintf "Bottleneck: %A" message)
                use client = new HttpClient()
                let content = new StringContent(JsonConvert.SerializeObject message)
                let! res = client.PostAsync("http://localhost:50552/api/cassandra", content) |> Async.AwaitTask
                let! body = res.Content.ReadAsStringAsync() |> Async.AwaitTask
                
                // post message to next step
                JsonConvert.DeserializeObject<Message> body
                |> Hydratated
                |> postMessage

        }) |> Agent // Agent is a constructor funtion that takes a MailboxProcessor. '|>' passes the mailboxprocessor to the constructor function
