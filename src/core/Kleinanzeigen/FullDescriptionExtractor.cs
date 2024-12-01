using HtmlAgilityPack;

namespace KleinanzeigenAdAlert.core.Kleinanzeigen;

public static class FullDescriptionExtractor
{
    public static async Task<string> GetDescriptionFromUrl(string adUrl)
    {
        var content = await PageContentRetriever.FetchPageContent(adUrl);
        var descriptionNode = GetDescription(content);
        var description = CleanString(descriptionNode);
        return description;
    }

    private static string GetDescription(string htmlContent)
    {
        if (htmlContent == null) throw new ArgumentNullException(nameof(htmlContent), "HTML content cannot be null.");

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(htmlContent);

        // Find the description node by its ID
        var descriptionNode = htmlDoc.DocumentNode.SelectSingleNode("//p[@id='viewad-description-text']");

        if (descriptionNode == null)
        {
            throw new InvalidOperationException("Could not find description node in the HTML content.");
        }

        return descriptionNode.InnerText;
    }

    private static string? CleanString(string? input)
    {
        if (input == null) return null;

        var result = input
            .Replace("\n", " ")
            .Replace("\r", "")
            .Replace("\t", "");

        while (result.Contains("  ")) // Double space
        {
            result = result.Replace("  ", " ");
        }

        return result.Trim();
    }
}