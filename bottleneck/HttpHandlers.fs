namespace bottleneck

module HttpHandlers =

    open Microsoft.AspNetCore.Http
    open FSharp.Control.Tasks.V2.ContextInsensitive
    open Giraffe
    open bottleneck.Models
    open System.Threading.Tasks

    let handleMessage =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let hydratedMessage message =
                    {   message with
                            Body = sprintf "%s and some info more" message.Body }

                let! messages = ctx.BindJsonAsync<Message list>()
                do! Task.Delay(100) // adding more treatment time
                let response = List.map hydratedMessage messages
                return! json response next ctx
            }

    open MessagesStore

    let messagesStore = MessagesStore ()

    let giveMessages (batchSize,total) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let messages = messagesStore.GetMessages batchSize total
                return! json messages next ctx
            }

    let messageTreated =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! ids = ctx.BindJsonAsync<int list>()
                messagesStore.MessageTreated ids
                let response = sprintf "Messages %A marked as treated" ids
                return! json response next ctx
            }

    let getMessagesInfo =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let treated = messagesStore.TreatedMessages ()
                let out = messagesStore.MessagesOut ()
                let response = sprintf "%i has been treated. %i messages still out" treated out
                return! json response next ctx
            }

    let resetMessages =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                messagesStore.Reset ()
                let response = sprintf "messages store cleared"
                return! json response next ctx
            }
