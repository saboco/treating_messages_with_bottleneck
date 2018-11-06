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

type Fetched = Fetched of RawMessage
type Hydratated = Hydratated of Message
type Zipped = Zipped of ZippedMessage
type Remove = Remove of int

type Agent<'a> = Agent of MailboxProcessor<'a>