[<RequireQualifiedAccess>]
module AgentsPool
open System
open Models

type AgentSelectorFunc<'a> = unit -> Async<Agent<'a>>
type StopAgentFunc<'a> = 'a -> Async<unit>

/// Generates a pool of CloudAgents of a specific size that can be used to select an agent when required.
let createAgentSelector<'a>(size, createAgent : unit -> Agent<'a>) : AgentSelectorFunc<'a> * StopAgentFunc<'a>=
    let agents =
        [ for _ in 1 .. size ->
            let (Agent agent) = createAgent ()
            try agent.Start()
            with _ -> () // it can fail with agent already started but we don't care

            Agent agent ]

    let r = Random()
    // function returned that will select an agent when needed
    (fun () -> async { return agents.[r.Next(0, agents.Length)] }), (fun msg ->  async {  for (Agent agent) in agents do agent.Post msg })