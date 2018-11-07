module String

open System

let trim (s : string) = if s = null then String.Empty else s.Trim()
let trimStart (c : char) (s : string) = if s = null then String.Empty else s.TrimStart c
let trimEnd (c : char) (s : string)  = if s = null then String.Empty else s.TrimEnd c

