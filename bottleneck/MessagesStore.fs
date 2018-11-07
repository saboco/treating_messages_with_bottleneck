module MessagesStore

open bottleneck.Models
open System.Collections.Concurrent

type MessagesStore () =

    let messagesTreated = new ConcurrentDictionary<int, sbyte>()
    let addMessagesTreated id = messagesTreated.AddOrUpdate(id, (fun _ -> 1y ), (fun _ v -> v))

    let mutable messagesOut : Set<int> = Set.empty
    let monitor = new System.Object()

    let mutable lastId : int = 0
    let updateLastId newId = lock monitor (fun () -> lastId <- newId)

    member __.GetMessages (batchSize : int) (total :int) =
        lock monitor (fun () ->
            if messagesTreated.Count + messagesOut.Count >= total  then
                ResizeArray<Message>(0)
            else
                let messages = ResizeArray<Message>(batchSize)
                for i in 1 .. batchSize do
                    let id = lastId + i
                    messages.Add({ Message.Id= id; Body = sprintf "This is a message with id %i" id })
                    messagesOut <- messagesOut.Add(id)

                updateLastId (lastId + batchSize)
                messages
        )

    member __.MessageTreated (ids : int list) =
        lock monitor (fun () ->
            ids
            |> List.map (fun id ->
                    addMessagesTreated id |> ignore
                    messagesOut <- messagesOut.Remove(id))) |> ignore

    member __.TreatedMessages () = messagesTreated.Count
    member __.MessagesOut () = lock monitor (fun () -> messagesOut.Count)
    member __.Reset () = lock monitor (fun () ->
       messagesOut <- Set.empty
       messagesTreated.Clear())
