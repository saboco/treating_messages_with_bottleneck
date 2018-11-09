[<RequireQualifiedAccess>]
module RawMessages
open Models
open System
open Newtonsoft.Json
open System.Net.Http

let private hasToBeTreated (message : RawMessage) =
    //let r = Random()
    //let n = r.Next(1,3)
    //message.Id % n = 0 // filter logic
    true

let fetchMessagesAgent (url : Uri) removeMsg getAgent numberOfMessages batchSizeToFetchMessages batchSizeToSendMessages numberOfAgents =
    let rec fetchMessages =
        let rec loop () =
            async {
                let url = Url.createUri url (sprintf "/batch/%i/total/%i" batchSizeToFetchMessages numberOfMessages)
                use client = new HttpClient()
                let! res = client.GetAsync(url) |> Async.AwaitTask
                let! body = res.Content.ReadAsStringAsync() |> Async.AwaitTask

                let messages = JsonConvert.DeserializeObject<RawMessage list> body
                let! (Agent agent) = getAgent ()

                messages
                |> List.filter hasToBeTreated
                |> List.chunkBySize batchSizeToSendMessages
                |> List.map Fetched
                |> List.map agent.Post
                |> ignore

                messages
                |> List.filter (hasToBeTreated >> not)
                |> List.map (fun message -> Remove message.Id)
                |> List.map removeMsg
                |> ignore

                if messages.Length > 0
                then return! loop ()
                else
                    printfn "*********************************************************************************************** RawMessages: no more messsages to fetch"
                    return ()
            }
        loop ()

    let agents = [for _ in 1 .. numberOfAgents do yield fetchMessages]

    agents
