
let nextMessage n = seq {
    for i in 1 .. n do
        yield sprintf "Message %i" i
}

let fetchMessages () =
    async {
        let nextMessage = nextMessage 5
        let enumerator = nextMessage.GetEnumerator()
        while enumerator.MoveNext() do
            let message = enumerator.Current
            printfn "%s" message
    }

fetchMessages () |> Async.RunSynchronously

let l = [for i in 1 .. 1 -> i]