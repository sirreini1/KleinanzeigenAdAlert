namespace KleinanzeigenAdAlert.models;

public class FlatAd(
    string location,
    string description,
    string price,
    string searchUrl,
    string adUrl,
    DateTime postedDate)
{
    public FlatAd(FlatAdDto details, string searchUrl) : this(details.Location, details.Description,
        details.Price, searchUrl, details.AdUrl, details.PostedDate)
    {
    }

    public string Location { get; set; } = location;
    public string AdUrl { get; set; } = adUrl;
    public string Description { get; set; } = description;
    public string Price { get; set; } = price;

    public string SearchUrl { get; set; } = searchUrl;
    public DateTime PostedDate { get; set; } = postedDate;
}