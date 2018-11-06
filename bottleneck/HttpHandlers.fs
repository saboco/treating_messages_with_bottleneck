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
                // 'let!' is like await a Task<T> and the result T is put into 'body' T is string here
                // var body = await ctx.ReadBodyFromRequestAsyn();
                let! message = ctx.BindJsonAsync<Message>()
                do! Task.Delay(1000); // do! is like await for a Task on c#
                let response = { 
                    message with
                        Body = sprintf "%s and some info more" message.Body }
                return! json response next ctx
            }