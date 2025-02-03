module Mvc.Sse

open System
open System.Text
open System.Threading.Tasks
open Microsoft.AspNetCore.Http

let setSseHeaders(response: HttpResponse) =
    response.Headers.Append("Cache-Control", "no-cache") |> ignore
    response.Headers.Append("Content-Type", "text/event-stream") |> ignore
    response.Headers.Append("Connection", "keep-alive") |> ignore
    response.Body.FlushAsync()

let sendServerSentEvent(
    response: HttpResponse,
    fragment: string,
    selector,
    mergeMode,
    settleDuration,
    useViewTransition,
    complete) =
    let selector = selector |> ValueOption.defaultValue ""
    let mergeMode = mergeMode |> ValueOption.defaultValue ""
    let settleDuration = settleDuration |> ValueOption.defaultValue 300
    let useViewTransition = useViewTransition |> ValueOption.defaultValue false
    let complete = complete |> ValueOption.defaultValue true

    task {
        // Clean the fragment by removing all newlines and extra spaces
        let fragment' =
            fragment
                .Replace(Environment.NewLine, "")
                .Replace("\n", "")
                .Replace("\r", "")
                .Trim()

        let data = StringBuilder("event: datastar-merge-fragments\n")

        if (not (String.IsNullOrEmpty selector)) then
            data.AppendLine($"data: selector {selector}") |> ignore

        if (not (String.IsNullOrEmpty mergeMode)) then
            data.AppendLine($"data: merge {mergeMode}") |> ignore

        if (settleDuration <> 300) then
            data.AppendLine($"data: settle {settleDuration}") |> ignore

        if useViewTransition then
            data.AppendLine("data: view-transition") |> ignore

        data.AppendLine($"data: fragments {fragment'}\n") |> ignore

        do! response.Body.WriteAsync(Encoding.UTF8.GetBytes(data.ToString()))
        do! response.Body.FlushAsync()

        if complete then response.Body.Close()
    }
    :> Task
