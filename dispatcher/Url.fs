module Url

open System

let createUri (url : Uri) (endpoint: string) =
    let trim = (String.trimStart '/' >> String.trimEnd '/')
    let baseEndpoint =  trim url.AbsolutePath
    let finalEndpoint =
        match baseEndpoint with
        | "" -> sprintf "/%s" (trim endpoint)
        | _ -> sprintf "/%s/%s" baseEndpoint (trim endpoint)

    Uri(url, finalEndpoint)