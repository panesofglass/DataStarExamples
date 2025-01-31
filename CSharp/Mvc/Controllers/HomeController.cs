using System.Diagnostics;
using System.Text.Json;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Mvc.Helpers;
using Mvc.Models;

namespace Mvc.Controllers;

public class HomeController : Controller
{
    private static List<Note> _notes;
    private static int _totalNoteCount;
    private readonly Random _random = new();
    
    public IActionResult Index()
    {
        if (_notes == null)
        {
            var count = _random.Next(25, 102);
            var todoFaker = new Faker<Note>()
                .RuleFor(t => t.Id, f => f.IndexFaker + 1)
                .RuleFor(t => t.Content, f => f.Lorem.Sentence());
            
            _notes = todoFaker.Generate(count).ToList();
            
            // Add 3 notes with "hello"
            for (var i = 0; i < 3; i++)
            {
                var randomIndex = _random.Next(_notes.Count);
                _notes[randomIndex] = new Note
                {
                    Id = _notes[randomIndex].Id,
                    Content = $"Hello! {new Faker().Lorem.Sentence()}"
                };
            }
            
            _totalNoteCount = _notes.Count;
        }

        ViewData["TotalCount"] = _totalNoteCount;
        var displayNotes = _notes.Take(5).ToList();
        ViewData["CurrentCount"] = displayNotes.Count;

        return View(displayNotes);
    }

    [Route("click-to-edit")]
    public IActionResult ClickToEdit()
    {
        return View();
    }

    [Route("bulk-update")]
    public IActionResult BulkUpdate()
    {
        return View();
    }

    [Route("click-to-load")]
    public IActionResult ClickToLoad()
    {
        return View();
    }

    [Route("delete-row")]
    public IActionResult DeleteRow()
    {
        return View();
    }

    [Route("edit-row")]
    public IActionResult EditRow()
    {
        return View();
    }

    [Route("lazy-load")]
    public IActionResult LazyLoad()
    {
        return View();
    }

    [Route("loading-indicator")]
    public IActionResult Indicator()
    {
        return View();
    }

    [Route("inline-validation")]
    public IActionResult InlineValidation()
    {
        return View();
    }

    [Route("infinite-scroll")]
    public IActionResult InfiniteScroll()
    {
        return View();
    }

    [Route("active-search")]
    public IActionResult ActiveSearch()
    {
        if (_notes == null)
        {
            var count = _random.Next(25, 102);
            var todoFaker = new Faker<Note>()
                .RuleFor(t => t.Id, f => f.IndexFaker + 1)
                .RuleFor(t => t.Content, f => f.Lorem.Sentence());

            _notes = todoFaker.Generate(count).ToList();

            // Add exactly 3 notes with "hello"
            for (var i = 0; i < 3; i++)
            {
                var randomIndex = _random.Next(_notes.Count);
                _notes[randomIndex] = new Note
                {
                    Id = _notes[randomIndex].Id,
                    Content = $"Hello! {new Faker().Lorem.Sentence()}"
                };
            }
            
            _totalNoteCount = _notes.Count;
        }

        ViewData["TotalCount"] = _totalNoteCount;
        var displayNotes = _notes.Take(5).ToList();
        ViewData["CurrentCount"] = displayNotes.Count;

        return View(displayNotes);
    }

    [HttpPut]
    [Route("search")]
    public async Task Search()
    {
        await SseHelper.SetSseHeadersAsync(Response);

        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();
        var query = body.Replace("{\"input\":\"", "").Replace("\"}", "");

        var filteredNotes = string.IsNullOrEmpty(query)
            ? _notes.Take(5).ToList()
            : _notes.Where(x => x.Content.Contains(query)).ToList();

        var countsHtml = filteredNotes.Count == 0
            ? $"<p id=\"total-count\">No results found out of {_totalNoteCount} notes</p>"
            : $"<p id=\"total-count\">Showing {filteredNotes.Count} of {_totalNoteCount} notes</p>";
        await SseHelper.SendServerSentEventAsync(Response, countsHtml);

        var notesListHtml = "<div id=\"notes-list\" class=\"notes-list\">";
        if (filteredNotes.Count == 0)
        {
            notesListHtml += "<div class=\"note-item\">";
            notesListHtml += $"<p>No notes found matching \"{query}\"</p>";
            notesListHtml += "</div>";
        }
        else
        {
            foreach (var note in filteredNotes)
            {
                notesListHtml += "<div class=\"note-item\">";
                notesListHtml += $"<p>{note.Content}</p>";
                notesListHtml += "</div>";
            }
        }

        notesListHtml += "</div>";

        await SseHelper.SendServerSentEventAsync(Response, notesListHtml);
    }

    [Route("progress-bar")]
    public IActionResult ProgressBar()
    {
        return View();
    }
    
    // [HttpGet("progress")]
    // public async Task Progress()
    // {
    //     await SseHelper.SetSseHeadersAsync(Response);
    //
    //     // picocss progress bar with min / max values
    //     var progressBarHtml = "<progress id=\"progress-bar\" value=\"0\" max=\"100\"></progress>";
    //     for (var i = 0; i <= 100; i++)
    //     {
    //         progressBarHtml += $"<progress id=\"progress-bar\" value=\"{i}\" max=\"100\"></progress>";
    //         await SseHelper.SendServerSentEventAsync(Response, progressBarHtml);
    //         await Task.Delay(100);
    //     }
    //     
    //     // picocss progress bar using intermediate flavor
    //     while (true)
    //     {
    //         var progressBarIntermediateHtml = $"<progress id=\"pprogress-bar-intermediate\" />";
    //         await SseHelper.SendServerSentEventAsync(Response, progressBarIntermediateHtml);
    //         await Task.Delay(100);
    //     }
    // }
    
    [HttpGet("progress")]
    public async Task Progress()
    {
        var type = Request.Query["type"];
        await SseHelper.SetSseHeadersAsync(Response);
        
        if (type == "intermediatse")
        {
            while (true)
            {
                var progressBarIntermediateHtml = "<progress id=\"progress-bar-intermediate\" />";
                await SseHelper.SendServerSentEventAsync(Response, progressBarIntermediateHtml);
                await Task.Delay(100);
            }
        }

        // Send a different progress bar HTML if type is not "intermediate"
        var progressBarHtml = "<progress id=\"progress-bar\" value=\"0\" max=\"100\"></progress>";
        for (var i = 0; i <= 100; i++)
        {
            progressBarHtml += $"<progress id=\"progress-bar\" value=\"{i}\" max=\"100\"></progress>";
            await SseHelper.SendServerSentEventAsync(Response, progressBarHtml);
            await Task.Delay(100);
        }
    }

    [Route("value-select")]
    public IActionResult ValueSelect()
    {
        return View();
    }

    [Route("animations")]
    public IActionResult Animations()
    {
        return View();
    }

    [Route("file-upload")]
    public IActionResult FileUpload()
    {
        return View();
    }

    [Route("dialogs-browser")]
    public IActionResult DialogsBrowser()
    {
        return View();
    }

    [Route("lazy-tabs")]
    public IActionResult LazyTabs()
    {
        return View();
    }

    [Route("sortable")]
    public IActionResult Sortable()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}