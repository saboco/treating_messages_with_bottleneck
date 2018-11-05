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
                let! body = ctx.ReadBodyFromRequestAsync()
                do! Task.Delay(1000); // do! is like await for a Task on c#
                let response = {
                    Text = sprintf "%s and some info more" body
                }
                return! json response next ctx
            }