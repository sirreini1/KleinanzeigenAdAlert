namespace KleinanzeigenAdAlert.core.Kleinanzeigen;

internal static class PageContentRetriever
{
    public static async Task<string> FetchPageContent(string url)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:128.0) Gecko/20100101 Firefox/128.0");
        request.Headers.Add("Accept", "*/*");
        request.Headers.Add("Accept-Language", "de,en-US;q=0.7,en;q=0.3");
        request.Headers.Add("Accept-Encoding", "gzip, deflate, br, zstd");
        request.Headers.Add("x-nextjs-data", "1");
        request.Headers.Add("Connection", "keep-alive");
        request.Headers.Add("Sec-Fetch-Dest", "empty");
        request.Headers.Add("Sec-Fetch-Mode", "cors");
        request.Headers.Add("Sec-Fetch-Site", "same-origin");
        request.Headers.Add("Priority", "u=4");
        request.Headers.Add("Pragma", "no-cache");
        request.Headers.Add("Cache-Control", "no-cache");
        request.Headers.Add("TE", "trailers");
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var contentStream = await response.Content.ReadAsStringAsync();
        return contentStream;
    }
}