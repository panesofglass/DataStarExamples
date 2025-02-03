module MinimalApi.NotesApi

open System
open System.IO
open System.Linq
open System.Text
open System.Threading.Tasks
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open MinimalApi

let get(context: HttpContext) =
    let noteService = context.RequestServices.GetService<NoteService>()

    task {
        do! Sse.setSseHeaders(context.Response)

        // For initial load, just take first 5 notes
        let! resp = noteService.Get()
        let filteredNotes = resp.Notes |> Seq.take 5

        let countsHtml =
            $"""<p id="stats">Showing <span class="number">{Seq.length filteredNotes}</span> of <span class="number">{resp.Count}</span> notes</p>"""
        do! Sse.sendServerSentEvent(context.Response, countsHtml, ValueOption.None, ValueOption.None, ValueOption.None, ValueOption.None, ValueOption.None)

        let notesListHtml = StringBuilder("""<div id="notes-list">""")
        for note in filteredNotes do
            notesListHtml
                .Append("""<span class="note-item">""")
                .Append($"<p>{note.Content}</p>")
                .Append("</span>") |> ignore

        notesListHtml.Append("</div>") |> ignore

        do! Sse.sendServerSentEvent(context.Response, notesListHtml.ToString(), ValueOption.None, ValueOption.None, ValueOption.None, ValueOption.None, ValueOption.None)
    }
    :> Task

let put(context: HttpContext) =
    task {
        do! Sse.setSseHeaders(context.Response)

        // Clear any existing error message first
        do! Sse.sendServerSentEvent(
            context.Response,
            "<span id=\"error\" class=\"error-message\" role=\"alert\" aria-live=\"polite\"></span>",
            ValueOption.None,ValueOption.None,ValueOption.None,ValueOption.None,ValueOption.None)

        use reader = new StreamReader(context.Request.Body)
        let! body = reader.ReadToEndAsync()
        let query = body.Replace("{\"input\":\"", "").Replace("\"}", "")

        // Check for minimum query length
        if query.Length < 3 && query.Length > 0 then
            let remainingChars = 3 - query.Length
            let characterPlural = if remainingChars = 1 then "character" else "characters"
            let errorHtml = $"<span id=\"error\" class=\"error-message\" role=\"alert\" aria-live=\"polite\">" +
                            $"Minimum search is 3 characters > please enter {remainingChars} more {characterPlural}</span>"
            do! Sse.sendServerSentEvent(
                context.Response, errorHtml,
                ValueOption.None,ValueOption.None,ValueOption.None,ValueOption.None,ValueOption.None)
        else
            let noteService = context.RequestServices.GetService<NoteService>()
            let! resp = noteService.Get()
            let notes = resp.Notes
            let totalNoteCount = resp.Count
            let filteredNotes =
                if String.IsNullOrEmpty(query) || query = "{}" then
                    notes.Take(5).ToList()
                else
                    notes.Where(_.Content.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList()

            let countsHtml =
                if filteredNotes.Count = 0 then
                    $"<p id=\"stats\">No results found for \"{query}\" out of <span class=\"number\">{totalNoteCount}</span> notes</p>"
                else $"<p id=\"stats\">Showing <span class=\"number\">{filteredNotes.Count}</span> of <span class=\"number\">{totalNoteCount}</span> notes</p>"
            do! Sse.sendServerSentEvent(
                context.Response, countsHtml,
                ValueOption.None, ValueOption.None,
                ValueOption.None, ValueOption.None,
                ValueOption.None)

            let notesListHtml = StringBuilder("""<div id="notes-list">""")
            if filteredNotes.Count = 0 then
                notesListHtml.Append(
                    $"""<span class="note-item"><p>No notes found matching "{query}"</p></span>""")
                |> ignore
            else
                for note in filteredNotes do
                    notesListHtml.Append(
                        $"""<span class="note-item"><p>{note.Content}</p></span>""")
                    |> ignore

            notesListHtml.Append("</div>") |> ignore

            do! Sse.sendServerSentEvent(
                context.Response, notesListHtml.ToString(),
                ValueOption.None, ValueOption.None,
                ValueOption.None, ValueOption.None,
                ValueOption.None)
    }
    :> Task
