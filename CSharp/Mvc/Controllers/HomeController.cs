using System.Diagnostics;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Mvc.Helpers;
using Mvc.Models;
using System.Text.Json;

namespace Mvc.Controllers;

public class HomeController : Controller
{
    private static List<Note> _notes;
    private static int _totalNoteCount;
    private readonly Random _random = new();

    public IActionResult Index()
    {
        return View();
    }

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
                    Content = $"hello! {new Faker().Lorem.Sentence()}"
                };
            }

            _totalNoteCount = _notes.Count;
        }

        ViewData["TotalCount"] = _totalNoteCount;
        var displayNotes = _notes.Take(5).ToList();
        ViewData["CurrentCount"] = displayNotes.Count;

        return View(displayNotes);
    }

    public IActionResult Animations()
    {
        return View();
    }

    public IActionResult BulkUpdate()
    {
        return View();
    }

    public IActionResult ClickToEdit()
    {
        return View();
    }

    public IActionResult ClickToLoad()
    {
        if (_notes == null)
        {
            var count = _random.Next(5, 15);
            var todoFaker = new Faker<Note>()
                .RuleFor(t => t.Id, f => f.IndexFaker + 1)
                .RuleFor(t => t.Content, f => f.Lorem.Sentence());

            _notes = todoFaker.Generate(count).ToList();

            // Add exactly 1 notes with "hello"
            for (var i = 0; i < 1; i++)
            {
                var randomIndex = _random.Next(_notes.Count);
                _notes[randomIndex] = new Note
                {
                    Id = _notes[randomIndex].Id,
                    Content = $"hello! {new Faker().Lorem.Sentence()}"
                };
            }

            _totalNoteCount = _notes.Count;
        }

        ViewData["TotalCount"] = _totalNoteCount;
        var displayNotes = _notes.Take(2).ToList();
        ViewData["CurrentCount"] = displayNotes.Count;

        return View(displayNotes);
    }

    public IActionResult DeleteRow()
    {
        return View();
    }

    public IActionResult DialogsBrowser()
    {
        return View();
    }

    public IActionResult EditRow()
    {
        return View();
    }

    public IActionResult FileUpload()
    {
        return View();
    }

    public IActionResult Indicator()
    {
        return View();
    }

    public IActionResult InfiniteScroll()
    {
        return View();
    }

    public IActionResult InlineValidation()
    {
        return View();
    }

    public IActionResult LazyLoad()
    {
        return View();
    }

    public IActionResult LazyTabs()
    {
        return View();
    }

    public IActionResult ProgressBar()
    {
        return View();
    }

    public IActionResult Sortable()
    {
        return View();
    }

    public IActionResult ValueSelect()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    #region Datastar Actions

    [HttpPut]
    public async Task Search()
    {
        await SseHelper.SetSseHeadersAsync(Response);

        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();
        var query = body.Replace("{\"input\":\"", "").Replace("\"}", "").Trim();

        var filteredNotes = string.IsNullOrEmpty(query)
            ? _notes.Take(5).ToList()
            : _notes.Where(x => x.Content.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();

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

    public async Task Progress()
    {
        var actionType = Request.Query["actionType"];

        var progressBarHtml = "<progress id=\"progressBar\" value=\"0\" max=\"100\" style=\"width: 100%;\"></progress>";
        var progressBarPercentageHtml =
            "<span id=\"progressBarPercentage\" style=\"position: absolute; left: 50%; top: 50%; transform: translate(-50%, -50%); font-weight: bold;\">0%</span>";

        if (actionType == "start")
        {
            for (var i = 0; i <= 100; i++)
            {
                progressBarHtml +=
                    $"<progress id=\"progressBar\" value=\"{i}\" max=\"100\" style=\"width: 100%;\"></progress>";
                progressBarPercentageHtml +=
                    $"<span id=\"progressBarPercentage\" style=\"position: absolute; left: 50%; top: 50%; transform: translate(-50%, -50%); font-weight: bold;\">{i}%</span>";

                await SseHelper.SendServerSentEventAsync(Response, progressBarHtml);
                await SseHelper.SendServerSentEventAsync(Response, progressBarPercentageHtml);
                await Task.Delay(100);
            }
        }
        else if (actionType == "repeat")
        {
            // TODO - Implement repeating progress bar
        }
        else if (actionType == "incremental")
        {
            // TODO - Implement incremental progress bar
        }
        else if (actionType == "reset")
        {
            // TODO - Implement reset progress bar
        }
    }

    // Endpoint that returns the image of a graph after delay
    public async Task LazyLoadImgGraph()
    {
        // Set SSE headers
        await SseHelper.SetSseHeadersAsync(Response);

        // Simulate initial delay
        await Task.Delay(2000);

        // Create HTML with transition and settling behavior
        const string graphHtml = @"
            <img id=""lazy-load""
                class=""transition-opacity""
                src=""/img/tokyo.png""
                alt=""Tokyo Graph"" />";

        // Send the HTML to the client
        await SseHelper.SendServerSentEventAsync(Response, graphHtml, null, null, 1000, false, false);
    }

    public async Task FetchIndicator()
    {
        await SseHelper.SetSseHeadersAsync(Response);
        var indicatorEmptyGreetingHtml = $"<p id=\"greeting\"></p>";
        await SseHelper.SendServerSentEventAsync(Response, indicatorEmptyGreetingHtml);
        await Task.Delay(2000);
        var indicatorGreetingHtml = $"<p id=\"greeting\">Hello, the time is {DateTimeOffset.UtcNow:O}</p>";
        await SseHelper.SendServerSentEventAsync(Response, indicatorGreetingHtml);
    }

    public async Task ClickToLoadMore()
    {
        try
        {
            // Set SSE headers
            await SseHelper.SetSseHeadersAsync(Response);

            // Check if the "datastar" query parameter exists
            if (HttpContext.Request.Query.ContainsKey("datastar"))
            {
                // Get the "datastar" query parameter values
                var json = HttpContext.Request.Query["datastar"].ToString();

                // DEBUG: Print the raw JSON string
                //Console.WriteLine($"Raw datastar value: {json}");

                // Deserialize the JSON string into a ClickToLoadSignals object
                var signals = JsonSerializer.Deserialize<ClickToLoadSignals>(json);

                // DEBUG: Print the deserialized signals
                //Console.WriteLine($"Deserialized signals: {signals.Offset}, {signals.Limit}");

                // DEBUG: Print the deserialized values
                // Console.WriteLine(signals != null
                //     ? $"Offset: {signals.Offset}, Limit: {signals.Limit}"
                //     : "Failed to deserialize datastar values");

                // get the filtered notes
                var filteredNotes = _notes
                    .Skip(signals.Offset)
                    .Take(signals.Limit)
                    .ToList();

                // update the total counts
                var totalCount = _notes.Count;
                var currentCount = Math.Min(signals.Offset + filteredNotes.Count, totalCount);
                var countHtml = $"<p id=\"total-count\">Showing {currentCount} of {totalCount} notes</p>";
                await SseHelper.SendServerSentEventAsync(Response, countHtml);

                // build the html for the new notes
                var notesHtml = "";
                foreach (var note in filteredNotes)
                {
                    notesHtml += $@"
                         <div class=""note-item"">
                             <p>{note.Content}</p>
                         </div>";
                }
                await SseHelper.SendServerSentEventAsync(Response, notesHtml, "#notes-list", "append", 1000, false, false);

                // Check if we've loaded all notes
                if (filteredNotes.Count == 0 || currentCount >= totalCount)
                {
                    var disabledButtonHtml = @"
                        <button 
                            id=""load-more-btn"" 
                            class=""button-disabled""
                            disabled>
                            No More Results
                        </button>";
                    await SseHelper.SendServerSentEventAsync(Response, disabledButtonHtml, "#load-more-btn", "outer");
                    return;
                }
            }
            else
            {
                Console.WriteLine("No datastar query parameters found.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    #endregion
}