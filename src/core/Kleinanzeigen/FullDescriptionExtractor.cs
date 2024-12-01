using System.Text.Json;
using System.Text.Json.Nodes;
using HtmlAgilityPack;

namespace KleinanzeigenAdAlert.core.Kleinanzeigen;

public static class FullDescriptionExtractor
{
    public static async Task<string> GetDescriptionFromUrl(string adUrl)
    {
        var content = await PageContentRetriever.FetchPageContent(adUrl);
        var descriptionNode = GetAdData(content);
        var description = CleanString(descriptionNode);
        return description;
    }

    private static string GetAdData(string htmlContent)
    {
        if (htmlContent == null) throw new ArgumentNullException(nameof(htmlContent), "HTML content cannot be null.");

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(htmlContent);

        // Find the seller name node
        var sellerNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'usercard--info')]//h2");
        var sellerName = sellerNode?.InnerText.Trim() ?? "Unknown Seller";

        // Find the title node
        var titleNode = htmlDoc.DocumentNode.SelectSingleNode("//h1[@id='viewad-title']");
        var title = titleNode?.InnerText.Trim() ?? "Unknown Title";
        // Find the description node by its ID
        var descriptionNode = htmlDoc.DocumentNode.SelectSingleNode("//p[@id='viewad-description-text']");

        if (descriptionNode == null)
        {
            throw new InvalidOperationException("Could not find description node in the HTML content.");
        }

        var data = new
        {
            adTitle = title,
            adOwner = sellerName,
            description = descriptionNode.InnerText
        };
        
        return JsonSerializer.Serialize(data);
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