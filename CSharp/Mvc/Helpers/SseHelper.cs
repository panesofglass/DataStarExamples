using System.Text;

namespace Mvc.Helpers;

public static class SseHelper
{
    public static async Task SetSseHeadersAsync(HttpResponse response)
    {
        response.Headers.Append("Cache-Control", "no-cache");
        response.Headers.Append("Content-Type", "text/event-stream");
        response.Headers.Append("Connection", "keep-alive");
        await response.Body.FlushAsync();
    }

    public static async Task SendServerSentEventAsync(HttpResponse response
        , string fragment = ""
        , string selector = ""
        , string mergeMode = ""
        , int settleDuration = 300
        , bool useViewTransition = false
        , bool end = false
        , string eventType = "datastar-merge-fragments"
    )
    {
        var data = $"event: {eventType}\n";

        if (!string.IsNullOrEmpty(selector))
        {
            data += $"data: selector {selector}\n";
        }

        if (!string.IsNullOrEmpty(fragment))
        {
            // Clean the fragment by removing all newlines and extra spaces
            fragment = fragment
                .Replace(Environment.NewLine, "")
                .Replace("\n", "")
                .Replace("\r", "")
                .Trim();

            data += $"data: fragments {fragment}\n";
        }

        if (!string.IsNullOrEmpty(mergeMode))
        {
            data += $"data: mergeMode {mergeMode}\n";
        }

        if (settleDuration != 300)
        {
            data += $"data: settleDuration {settleDuration}\n";
        }

        if (useViewTransition)
        {
            data += $"data: useViewTransition {useViewTransition}\n";
        }

        data += "\n";

        await response.Body.WriteAsync(Encoding.UTF8.GetBytes(data));
        await response.Body.FlushAsync();

        if (end)
        {
            response.Body.Close();
        }
    }
}