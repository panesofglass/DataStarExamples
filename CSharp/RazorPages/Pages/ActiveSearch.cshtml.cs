using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPages.Helpers;
using RazorPages.Models;

namespace RazorPages.Pages;

public class ActiveSearch : PageModel
{
    private static List<Note> _notes;
    private static int _totalNoteCount;
    private readonly Random _random = new();

    [BindProperty] public List<Note> Notes { get; set; }

    public void OnGet()
    {
        if (_notes == null)
        {
            var count = _random.Next(20, 983);
            var todoFaker = new Faker<Note>()
                .RuleFor(t => t.Id, f => f.IndexFaker + 1)
                .RuleFor(t => t.Content, f => f.Lorem.Sentence());

            _notes = todoFaker.Generate(count);
            _totalNoteCount = _notes.Count;
        }

        ViewData["TotalCount"] = _totalNoteCount;
        Notes = _notes.Take(5).ToList();
        ViewData["CurrentCount"] = Notes.Count;
    }

    public async Task OnPutSearchAsync()
    {
        await SseHelper.SetSseHeaders(Response);

        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();
        var query = body.Replace("{\"input\":\"", "").Replace("\"}", "");

        var filteredNotes = string.IsNullOrEmpty(query)
            ? _notes.Take(5).ToList()
            : _notes.Where(x => x.Content.Contains(query)).ToList();

        // Send the total count of notes and the count of notes being displayed
        var countsHtml = filteredNotes.Count == 0
            ? $"<p id=\"total-count\">No results found for \"{query}\" out of {_totalNoteCount} notes</p>"
            : $"<p id=\"total-count\">Showing {filteredNotes.Count} of {_totalNoteCount} notes</p>";
        await SseHelper.SendServerSentEvent(Response, countsHtml);

        // Send the notes list
        var notesListHtml = "<div id=\"notes-list\">";
        if (filteredNotes.Count == 0)
        {
            notesListHtml += "<span class=\"note-item\">";
            notesListHtml += $"<p>No notes found matching \"{query}\"</p>";
            notesListHtml += "</span>";
        }
        else
        {
            foreach (var note in filteredNotes)
            {
                notesListHtml += "<span class=\"note-item\">";
                notesListHtml += $"<p>{note.Content}</p>";
                notesListHtml += "</span>";
            }
        }

        notesListHtml += "</div>";

        await SseHelper.SendServerSentEvent(Response, notesListHtml);
    }
}