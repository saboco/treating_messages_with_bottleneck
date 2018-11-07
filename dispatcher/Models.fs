module Models


[<CLIMutable>]
type RawMessage = {
    Id : int
    Body : string }

[<CLIMutable>]
type Message = {
    Id : int
    Body : string }

type ZippedMessage =  {
    Id : int
    Body : string
}

type Fetched =
    | Fetched of RawMessage list
    | Stop

type Hydratated =
    | Hydratated of Message
    | Stop

type Zipped =
    | Zipped of ZippedMessage
    | Stop
type Remove =
    | Remove of int
    | Stop

type Agent<'a> = Agent of MailboxProcessor<'a>