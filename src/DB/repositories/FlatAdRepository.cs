using KleinanzeigenAdAlert.DB.entities;
using KleinanzeigenAdAlert.models;

namespace KleinanzeigenAdAlert.DB.repositories;

public interface IFlatAdRepository
{
    List<FlatAd> GetFlatAds();
    List<FlatAd> GetFlatAdsForUser(string telegramUser);
    void DeleteFlatAdsForUserAndUrl(string telegramUser, string searchUrl);
    List<TelegramUserSearchUrlPair> GetUniqueTelegramUserSearchUrlPairs();
    void DeleteOldFlatAds(int maxAgeInDays = 7, int minNumberOfAds = 25);
    void DeleteFlatAd(string id);
    void DeleteAllFlatAds();
    void UpsertFlatAd(FlatAd flatAd, string telegramUser);
    List<FlatAd> CheckForNewAds(List<FlatAd> flatAds, string telegramUser);
    bool EntriesForURlAndUserExist(string searchUrl, string telegramUser);
    string GetStatisticPerUser(string telegramUser);
    void UpsertFlatAds(List<FlatAd> flatAds, string telegramUser);
}

public class FlatAdRepository : IFlatAdRepository
{
    private readonly AppDbContext _dbContext;

    public FlatAdRepository(AppDbContext _dbContext)
    {
        this._dbContext = _dbContext;
    }

    public List<FlatAd> GetFlatAds()
    {
        return _dbContext.FlatAds.Select(e => FlatAdEntity.ToFlatAd(e)).ToList();
    }

    public List<FlatAd> GetFlatAdsForUser(string telegramUser)
    {
        return GetFlatAdEntitiesForUser(telegramUser).Select(FlatAdEntity.ToFlatAd)
            .ToList();
    }

    private List<FlatAdEntity> GetFlatAdEntitiesForUser(string telegramUser)
    {
        return _dbContext.FlatAds.Where(e => e.TelegramUser == telegramUser).ToList();
    }

    public void DeleteFlatAdsForUserAndUrl(string telegramUser, string searchUrl)
    {
        _dbContext.FlatAds.RemoveRange(_dbContext.FlatAds.Where(e =>
            e.TelegramUser == telegramUser && e.SearchUrl == searchUrl));
        _dbContext.SaveChanges();
    }

    public List<TelegramUserSearchUrlPair> GetUniqueTelegramUserSearchUrlPairs()
    {
        return _dbContext.FlatAds
            .GroupBy(flatAdEntity => new { flatAdEntity.TelegramUser, flatAdEntity.SearchUrl })
            .Select(group => new TelegramUserSearchUrlPair(group.Key.TelegramUser, group.Key.SearchUrl))
            .ToList();
    }

    public void DeleteOldFlatAds(int maxAgeInDays = 7, int minNumberOfAds = 25)
    {
        var allUserIdsWithAds = _dbContext.FlatAds.Select(ad => ad.TelegramUser).Distinct().ToList();
        foreach (var userId in allUserIdsWithAds)
        {
            var userAds = GetFlatAdEntitiesForUser(userId);
            if (userAds.Count > minNumberOfAds)
            {
                var oldAds = userAds.Where(e => e.PostedDate < DateTime.Now.AddDays(-maxAgeInDays));
                _dbContext.FlatAds.RemoveRange(oldAds);
            }
        }

        _dbContext.SaveChanges();
    }

    public void DeleteFlatAd(string id)
    {
        var flatAd = _dbContext.FlatAds.Find(id);
        if (flatAd != null)
        {
            _dbContext.FlatAds.Remove(flatAd);
            _dbContext.SaveChanges();
        }
    }

    public void DeleteAllFlatAds()
    {
        _dbContext.FlatAds.RemoveRange(_dbContext.FlatAds);
        _dbContext.SaveChanges();
    }

    public void UpsertFlatAd(FlatAd flatAd, string telegramUser)
    {
        var entity = FlatAdEntity.FromFlatAd(flatAd, telegramUser);
        var existingAd = _dbContext.FlatAds.Find(entity.Id);
        if (existingAd != null)
            _dbContext.Entry(existingAd).CurrentValues.SetValues(entity);
        else
            _dbContext.FlatAds.Add(entity);

        _dbContext.SaveChanges();
    }

    public List<FlatAd> CheckForNewAds(List<FlatAd> flatAds, string telegramUser)
    {
        var newAds = new List<FlatAd>();
        foreach (var flatAd in flatAds)
        {
            var entity = FlatAdEntity.FromFlatAd(flatAd, telegramUser);
            var existingAd = _dbContext.FlatAds.Find(entity.Id);
            if (existingAd == null) newAds.Add(flatAd);
        }

        return newAds;
    }

    public bool EntriesForURlAndUserExist(string searchUrl, string telegramUser)
    {
        return _dbContext.FlatAds.Any(e => e.SearchUrl == searchUrl && e.TelegramUser == telegramUser);
    }

    public string GetStatisticPerUser(string telegramUser)
    {
        var userAds = _dbContext.FlatAds.Where(e => e.TelegramUser == telegramUser).ToList();
        var uniqueSearchUrls = userAds.Select(ad => ad.SearchUrl).Distinct().ToList();
        var message = $"You are watching {uniqueSearchUrls.Count} unique search URLs. With:\n\n";

        for (var i = 0; i < uniqueSearchUrls.Count; i++)
        {
            var url = uniqueSearchUrls[i];
            var adsForUrl = userAds.Where(ad => ad.SearchUrl == url).ToList();
            message += $"{adsForUrl.Count} ads for url with id: {i}\n";
        }

        return message;
    }

    public void UpsertFlatAds(List<FlatAd> flatAds, string telegramUser)
    {
        foreach (var flatAd in flatAds)
        {
            var entity = FlatAdEntity.FromFlatAd(flatAd, telegramUser);
            var existingAd = _dbContext.FlatAds.Find(entity.Id);
            if (existingAd != null)
                _dbContext.Entry(existingAd).CurrentValues.SetValues(entity);
            else
                _dbContext.FlatAds.Add(entity);
        }

        _dbContext.SaveChanges();
    }
}