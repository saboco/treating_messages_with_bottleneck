[<RequireQualifiedAccess>]
module RawMessages
open Models
open System

let private nextMessage n : seq<RawMessage> = seq {
    for i in 1 .. n do
        yield { Id = i; Body = sprintf "Message %i" i }
}

let private hasToBeTreated (message : RawMessage) =
    let r = Random()
    let n = r.Next(1,3)
    message.Id % n = 0 // filter logic

let fetchMessagesAgent removeMsg getAgent n =
    async {
    let nextMessage = nextMessage n
    let enumerator = nextMessage.GetEnumerator()
    while enumerator.MoveNext() do
        let message = enumerator.Current
        if hasToBeTreated message  then
            let! (Agent agent) = getAgent ()
            message
            |> Fetched
            |> agent.Post
        else
            message.Id
            |> Remove
            |> removeMsg
    }
