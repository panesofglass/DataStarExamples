namespace Mvc.Controllers

open System
open System.Diagnostics
open System.IO
open System.Linq
open System.Text

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging

open Mvc
open Mvc.Models
open Mvc.Services
open System.Threading.Tasks

type HomeController (noteService: NoteService, logger : ILogger<HomeController>) =
    inherit Controller()

    member this.Index () =
        task {
            let! resp = noteService.Get()
            this.ViewData["TotalCount"] <- resp.Count
            let displayNotes = resp.Notes.Take(5).ToList()
            this.ViewData["CurrentCount"] <- displayNotes.Count
            return this.View(displayNotes)
        }
    
    [<Route("active-search")>]
    member this.ActiveSearch() =
        task {
            let! resp = noteService.Get()
            this.ViewData["TotalCount"] <- resp.Count
            let displayNotes = resp.Notes.Take(5).ToList()
            this.ViewData["CurrentCount"] <- displayNotes.Count
            return this.View(displayNotes)
        }

    [<HttpPut; Route("search")>]
    member this.Search() =
        task {
            do! Sse.setSseHeaders(this.Response)

            // Clear any existing error message first
            do! Sse.sendServerSentEvent(
                this.Response,
                "<span id=\"error\" class=\"error-message\" role=\"alert\" aria-live=\"polite\"></span>",
                ValueOption.None,ValueOption.None,ValueOption.None,ValueOption.None,ValueOption.None)

            use reader = new StreamReader(this.Request.Body)
            let! body = reader.ReadToEndAsync()
            let query = body.Replace("{\"input\":\"", "").Replace("\"}", "")

            // Check for minimum query length
            if query.Length < 3 && query.Length > 0 then
                let remainingChars = 3 - query.Length
                let characterPlural = if remainingChars = 1 then "character" else "characters"
                let errorHtml = $"<span id=\"error\" class=\"error-message\" role=\"alert\" aria-live=\"polite\">" +
                                $"Minimum search is 3 characters > please enter {remainingChars} more {characterPlural}</span>"
                do! Sse.sendServerSentEvent(
                    this.Response, errorHtml,
                    ValueOption.None,ValueOption.None,ValueOption.None,ValueOption.None,ValueOption.None)
            else
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
                    this.Response, countsHtml,
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
                    this.Response, notesListHtml.ToString(),
                    ValueOption.None, ValueOption.None,
                    ValueOption.None, ValueOption.None,
                    ValueOption.None)
        } :> Task

    [<ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)>]
    member this.Error () =
        let reqId = 
            if isNull Activity.Current then
                this.HttpContext.TraceIdentifier
            else
                Activity.Current.Id

        this.View({ RequestId = reqId })
