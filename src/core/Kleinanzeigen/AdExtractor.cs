using System.Globalization;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using KleinanzeigenAdAlert.models;

namespace KleinanzeigenAdAlert.core.Kleinanzeigen;

public static partial class AdExtractor
{
    public static async Task<List<FlatAd>> GetAdsFromUrl(string searchUrl)
    {
        var content = await PageContentRetriever.FetchPageContent(searchUrl);
        var adNodes = GetListingNodes(content);
        var flatAdDtos = adNodes.Select(ExtractAdDetails).ToList();
        return flatAdDtos.Select(dto => new FlatAd(dto, searchUrl)).ToList();
    }

    private static List<HtmlNode> GetListingNodes(string htmlContent)
    {
        if (htmlContent == null) throw new ArgumentNullException(nameof(htmlContent), "HTML content cannot be null.");

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(htmlContent);

        // The listings are in article elements with class "aditem"
        var listingNodes = htmlDoc.DocumentNode.SelectNodes("//article[@class='aditem']")
            ?.Where(node => !node.InnerHtml.Contains("liberty-position-name")) // Filter out ads
            .ToList() ?? new List<HtmlNode>();

        return listingNodes;
    }

    private static FlatAdDto ExtractAdDetails(HtmlNode node)
    {
        // Extract price
        var priceElement = node.SelectSingleNode(".//p[@class='aditem-main--middle--price-shipping--price']");
        var priceText = priceElement?.InnerText.Trim() ?? "Price not found";
        var price = $"{PriceRegex().Replace(priceText, string.Empty)}€";

        // Extract location
        var locationElement = node.SelectSingleNode(".//div[@class='aditem-main--top--left']");
        var locationText = locationElement?.InnerText.Trim() ?? "Location not found";
        // Remove the icon text and clean up the location
        var location = CleanString(locationText);

        // Extract description
        var descriptionElement = node.SelectSingleNode(".//p[@class='aditem-main--middle--description']");
        var description = CleanString(descriptionElement?.InnerText) ?? "Description not found";

        // Extract URL
        var urlElement = node.SelectSingleNode(".//div[@class='aditem-image']//a");
        var adUrl =
            $"{Config.SearchBaseUrl}{urlElement?.GetAttributeValue("href", "URL not found").Trim()}";

        var dateElement = node.SelectSingleNode(".//div[@class='aditem-main--top--right']");
        var dateText = CleanString(dateElement?.InnerText) ?? "Date not found";
        dateText = dateText.Replace("i", "").Trim();
        DateTime postedDate;
        try
        {
            postedDate = ParseGermanDateTime(dateText);
        }
        catch (Exception)
        {
            postedDate = DateTime.Now; // or another appropriate default value
        }

        return new FlatAdDto(
            location,
            description,
            price,
            adUrl,
            postedDate
        );
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

    private static DateTime ParseGermanDateTime(string dateText)
    {
        dateText = dateText.ToLower(); // Normalize to lowercase for easier comparison
        var now = DateTime.Now;

        // Handle "Heute" (today)
        if (dateText.StartsWith("heute"))
        {
            var timeStr = dateText.Split(',')[1].Trim();
            var time = TimeSpan.ParseExact(timeStr, "hh\\:mm", CultureInfo.InvariantCulture);
            return now.Date.Add(time);
        }

        // Handle "Gestern" (yesterday)
        if (dateText.StartsWith("gestern"))
        {
            var timeStr = dateText.Split(',')[1].Trim();
            var time = TimeSpan.ParseExact(timeStr, "hh\\:mm", CultureInfo.InvariantCulture);
            return now.Date.AddDays(-1).Add(time);
        }

        // Handle full date format (e.g., "25.11.2024")
        return DateTime.ParseExact(dateText, "dd.MM.yyyy", CultureInfo.InvariantCulture);
    }

    [GeneratedRegex(@"[^0-9,.]")] // Modified to keep only numbers and decimal separators
    private static partial Regex PriceRegex();

    [GeneratedRegex(@"\d{5}")] // German zip codes are also 5 digits
    private static partial Regex ZipCodeRegex();
}