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
                // 'let!' is like await a Task<T> and the result T is put into 'body'
                // var body = await ctx.BindJsonAsync<Message>()
                let! message = ctx.BindJsonAsync<Message>()
                // do! is like await for a Task on c#
                // await Task.Delay(1000);
                do! Task.Delay(1000)
                let response = {
                    message with
                        Body = sprintf "%s and some info more" message.Body }
                return! json response next ctx
            }