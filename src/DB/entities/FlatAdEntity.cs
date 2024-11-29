using System.ComponentModel.DataAnnotations;
using KleinanzeigenAdAlert.models;

namespace KleinanzeigenAdAlert.DB.entities;

public class FlatAdEntity(
    string location,
    string description,
    string price,
    string searchUrl,
    string adUrl,
    string telegramUser,
    DateTime postedDate,
    string Id = "")

{
    //For some reason we need this?
    public FlatAdEntity() : this("", "", "", "", "", "", DateTime.Now)
    {
    }

    [Key] [MaxLength(1000)] public string Id { get; init; } = GenerateId(telegramUser, adUrl);
    [MaxLength(1000)] public string AdUrl { get; init; } = adUrl;

    [MaxLength(256)] public string TelegramUser { get; init; } = telegramUser;

    [MaxLength(255)] public string Location { get; init; } = location;
    [MaxLength(1000)] public string Description { get; init; } = description;
    [MaxLength(10)] public string Price { get; init; } = price;
    [MaxLength(1000)] public string SearchUrl { get; init; } = searchUrl;
    public DateTime PostedDate { get; init; } = postedDate;

    public static FlatAdEntity FromFlatAd(FlatAd flatAd, string telegramUser)
    {
        return new FlatAdEntity(flatAd.Location, flatAd.Description, flatAd.Price, flatAd.SearchUrl, flatAd.AdUrl,
            telegramUser, flatAd.PostedDate);
    }

    public static string GenerateId(string telegramUser, string adUrl)
    {
        return $"{telegramUser}_{adUrl}";
    }


    public static FlatAd ToFlatAd(FlatAdEntity flatAdEntity)
    {
        return new FlatAd(flatAdEntity.Location, flatAdEntity.Description, flatAdEntity.Price, flatAdEntity.SearchUrl,
            flatAdEntity.AdUrl, flatAdEntity.PostedDate);
    }
}