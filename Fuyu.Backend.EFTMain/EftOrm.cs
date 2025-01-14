using System;
using System.Collections.Generic;
using Fuyu.Backend.BSG.Models.Accounts;
using Fuyu.Backend.BSG.Models.Customization;
using Fuyu.Backend.BSG.Models.Locations;
using Fuyu.Backend.BSG.Models.Profiles;
using Fuyu.Backend.BSG.Models.Profiles.Info;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.BSG.Models.Trading;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.EFTMain;

public class EftOrm
{
    public static EftOrm Instance => instance.Value;
    private static readonly Lazy<EftOrm> instance = new(() => new EftOrm());

    private readonly EftDatabase _eftDatabase;

    /// <summary>
    /// The construction of this class is handled in the <see cref="instance"/> (<see cref="Lazy{T}"/>)
    /// </summary>
    private EftOrm()
    {
        _eftDatabase = EftDatabase.Instance;
    }

    #region Profile
    public List<EftProfile> GetProfiles()
    {
        return _eftDatabase.Profiles.ToList();
    }

    public EftProfile GetProfile(string profileId)
    {
        var profiles = GetProfiles();

        for (var i = 0; i < profiles.Count; ++i)
        {
            if (profiles[i].Pmc._id == profileId)
            {
                return profiles[i];
            }
        }

        return null;
    }

    public EftProfile GetActiveProfile(string sessionId)
    {
        return GetActiveProfile(GetAccount(sessionId));
    }

    public EftProfile GetActiveProfile(EftAccount account)
    {
        var profiles = GetProfiles();
        string profileId = null;

        switch (account.CurrentSession)
        {
            case ESessionMode.Regular:
                profileId = account.PvpId;
                break;

            case ESessionMode.Pve:
                profileId = account.PveId;
                break;

            default:
                throw new Exception($"Unhandled session mode: {account.CurrentSession}");
        }

        foreach (var profile in profiles)
        {
            if (profile.Pmc._id == profileId)
            {
                return profile;
            }
        }

        return null;
    }

    public void SetOrAddProfile(EftProfile profile)
    {
        var profiles = GetProfiles();

        for (var i = 0; i < profiles.Count; ++i)
        {
            if (profiles[i].Pmc._id == profile.Pmc._id)
            {
                _eftDatabase.Profiles.TrySet(i, profile);
                return;
            }
        }

        _eftDatabase.Profiles.Add(profile);
    }

    public void RemoveProfile(EftProfile profile)
    {
        var profiles = GetProfiles();

        for (var i = 0; i < profiles.Count; ++i)
        {
            if (profiles[i].Pmc._id == profile.Pmc._id)
            {
                _eftDatabase.Profiles.TryRemoveAt(i);
                return;
            }
        }
    }
    #endregion

    #region Account
    public List<EftAccount> GetAccounts()
    {
        return _eftDatabase.Accounts.ToList();
    }

    public EftAccount GetAccount(int accountId)
    {
        var accounts = GetAccounts();

        for (var i = 0; i < accounts.Count; ++i)
        {
            if (accounts[i].Id == accountId)
            {
                return accounts[i];
            }
        }

        return null;
    }

    public EftAccount GetAccount(string sessionId)
    {
        var accountId = GetSession(sessionId);
        return GetAccount(accountId);
    }

    public void SetOrAddAccount(EftAccount account)
    {
        var accounts = GetAccounts();

        for (var i = 0; i < accounts.Count; ++i)
        {
            if (accounts[i].Id == account.Id)
            {
                _eftDatabase.Accounts.TrySet(i, account);
                return;
            }
        }

        _eftDatabase.Accounts.Add(account);
    }

    public void RemoveAccount(EftAccount account)
    {
        var accounts = GetAccounts();

        for (var i = 0; i < accounts.Count; ++i)
        {
            if (accounts[i].Id == account.Id)
            {
                _eftDatabase.Accounts.TryRemoveAt(i);
                return;
            }
        }
    }
    #endregion

    #region Session
    public Dictionary<string, int> GetSessions()
    {
        return _eftDatabase.Sessions.ToDictionary();
    }

    public int GetSession(string sessionId)
    {
        if (!_eftDatabase.Sessions.TryGet(sessionId, out var session))
        {
            throw new Exception($"Failed to get session from sessionId: {sessionId}");
        }

        return session;
    }

    public void SetOrAddSession(string sessionId, int accountId)
    {
        if (_eftDatabase.Sessions.ContainsKey(sessionId))
        {
            _eftDatabase.Sessions.Set(sessionId, accountId);
        }
        else
        {
            _eftDatabase.Sessions.Set(sessionId, accountId);
        }
    }

    public void RemoveSession(string sessionId)
    {
        _eftDatabase.Sessions.Remove(sessionId);
    }
    #endregion

    #region Customization
    public Dictionary<string, CustomizationTemplate> GetCustomizations()
    {
        return _eftDatabase.Customizations.ToDictionary();
    }

    public CustomizationTemplate GetCustomization(string customizationId)
    {
        if (!_eftDatabase.Customizations.TryGet(customizationId, out var customizationTemplate))
        {
            throw new Exception($"Failed to get customization with ID '{customizationId}'");
        }

        return customizationTemplate;
    }

    public void SetOrAddCustomization(string customizationId, CustomizationTemplate template)
    {
        if (_eftDatabase.Customizations.ContainsKey(customizationId))
        {
            _eftDatabase.Customizations.Set(customizationId, template);
        }
        else
        {
            _eftDatabase.Customizations.Set(customizationId, template);
        }
    }

    public void RemoveCustomization(string customizationId)
    {
        _eftDatabase.Customizations.Remove(customizationId);
    }
    #endregion

    #region Customization storage
    public List<CustomizationStorageEntry> GetCustomizationStorage()
    {
        return _eftDatabase.CustomizationStorage.ToList();
    }

    public CustomizationStorageEntry GetCustomizationStorage(string customizationId)
    {
        var customization = GetCustomizationStorage();

        for (var i = 0; i < customization.Count; ++i)
        {
            if (customization[i].Id == customizationId)
            {
                return customization[i];
            }
        }

        return null;
    }

    public void SetOrAddCustomizationStorage(CustomizationStorageEntry entry)
    {
        var customization = GetCustomizationStorage();

        for (var i = 0; i < customization.Count; ++i)
        {
            if (customization[i].Id == entry.Id)
            {
                _eftDatabase.CustomizationStorage.TrySet(i, entry);
                return;
            }
        }

        _eftDatabase.CustomizationStorage.Add(entry);
    }

    public void RemoveCustomizationStorage(CustomizationStorageEntry entry)
    {
        var customization = GetCustomizationStorage();

        for (var i = 0; i < customization.Count; ++i)
        {
            if (customization[i].Id == entry.Id)
            {
                _eftDatabase.CustomizationStorage.TryRemoveAt(i);
                return;
            }
        }
    }
    #endregion

    #region Languages
    public Dictionary<string, string> GetLanguages()
    {
        return _eftDatabase.Languages.ToDictionary();
    }

    public string GetLanguage(string languageId)
    {
        if (!_eftDatabase.Languages.TryGet(languageId, out var language))
        {
            throw new Exception($"Failed to get language from languageId: {languageId}");
        }

        return language;
    }

    public void SetOrAddLanguage(string languageId, string name)
    {
        if (_eftDatabase.Languages.ContainsKey(languageId))
        {
            _eftDatabase.Languages.Set(languageId, name);
        }
        else
        {
            _eftDatabase.Languages.Set(languageId, name);
        }
    }

    public void RemoveLanguage(string languageId)
    {
        _eftDatabase.Languages.Remove(languageId);
    }
    #endregion

    #region GlobalLocales
    public Dictionary<string, Dictionary<string, string>> GetGlobalLocales()
    {
        return _eftDatabase.GlobalLocales.ToDictionary();
    }

    public Dictionary<string, string> GetGlobalLocale(string languageId)
    {
        if (!_eftDatabase.GlobalLocales.TryGet(languageId, out var locale))
        {
            throw new Exception($"Failed to get locale '{languageId}'");
        }

        return locale;
    }

    public void SetOrAddGlobalLocale(string languageId, Dictionary<string, string> globalLocale)
    {
        if (_eftDatabase.GlobalLocales.ContainsKey(languageId))
        {
            _eftDatabase.GlobalLocales.Set(languageId, globalLocale);
        }
        else
        {
            _eftDatabase.GlobalLocales.Set(languageId, globalLocale);
        }
    }

    public void RemoveGlobalLocale(string languageId)
    {
        _eftDatabase.GlobalLocales.Remove(languageId);
    }
    #endregion

    #region MenuLocales
    public Dictionary<string, MenuLocaleResponse> GetMenuLocales()
    {
        return _eftDatabase.MenuLocales.ToDictionary();
    }

    public MenuLocaleResponse GetMenuLocale(string languageId)
    {
        if (!_eftDatabase.MenuLocales.TryGet(languageId, out var menuLocale))
        {
            throw new Exception($"Failed to get menu locale from languageId: {languageId}");
        }

        return menuLocale;
    }

    public void SetOrAddMenuLocale(string languageId, MenuLocaleResponse menuLocale)
    {
        if (_eftDatabase.MenuLocales.ContainsKey(languageId))
        {
            _eftDatabase.MenuLocales.Set(languageId, menuLocale);
        }
        else
        {
            _eftDatabase.MenuLocales.Set(languageId, menuLocale);
        }
    }

    public void RemoveMenuLocale(string languageId)
    {
        _eftDatabase.MenuLocales.Remove(languageId);
    }
    #endregion

    #region Default builds
    public BuildsListResponse GetDefaultBuilds()
    {
        return _eftDatabase.DefaultBuilds.Get();
    }

    public void SetDefaultBuilds(BuildsListResponse builds)
    {
        _eftDatabase.DefaultBuilds.Set(builds);
    }
    #endregion

    #region Wipe profiles
    public Dictionary<string, Dictionary<EPlayerSide, Profile>> GetWipeProfiles()
    {
        return _eftDatabase.WipeProfiles.ToDictionary();
    }

    public Dictionary<EPlayerSide, Profile> GetWipeProfile(string edition)
    {
        if (!_eftDatabase.WipeProfiles.TryGet(edition, out var profiles))
        {
            throw new Exception($"Failed to get profile(s) for edition '{edition}'");
        }

        return profiles;
    }

    public void SetOrAddWipeProfile(string edition, Dictionary<EPlayerSide, Profile> profiles)
    {
        if (_eftDatabase.WipeProfiles.ContainsKey(edition))
        {
            _eftDatabase.WipeProfiles.Set(edition, profiles);
        }
        else
        {
            _eftDatabase.WipeProfiles.Set(edition, profiles);
        }
    }

    public void RemoveWipeProfile(string edition)
    {
        _eftDatabase.WipeProfiles.Remove(edition);
    }
    #endregion

    #region WorldMap
    // TODO: parse from model
    // -- seionmoya, 2024-01-09
    public JObject GetWorldMap()
    {
        return _eftDatabase.WorldMap.Get();
    }

    // TODO: parse from model
    // -- seionmoya, 2024-01-09
    public void SetWorldMap(JObject worldmap)
    {
        _eftDatabase.WorldMap.Set(worldmap);
    }
    #endregion

    #region Hideout settings
    public HideoutSettingsResponse GetHideoutSettings()
    {
        return _eftDatabase.HideoutSettings.Get();
    }

    public void SetHideoutSettings(HideoutSettingsResponse settings)
    {
        _eftDatabase.HideoutSettings.Set(settings);
    }
    #endregion

    #region Achievement statistic
    public AchievementStatisticResponse GetAchievementStatistics()
    {
        return _eftDatabase.AchievementStatistic.Get();
    }

    public void SetAchievementStatistics(AchievementStatisticResponse statistics)
    {
        _eftDatabase.AchievementStatistic.Set(statistics);
    }
    #endregion

    #region Unparsed
    public JObject GetAchievements()
    {
        return _eftDatabase.Achievements.Get();
    }

    public void SetAchievements(JObject achievements)
    {
        _eftDatabase.Achievements.Set(achievements);
    }

    public JObject GetGlobals()
    {
        return _eftDatabase.Globals.Get();
    }

    public void SetGlobals(JObject globals)
    {
        _eftDatabase.Globals.Set(globals);
    }

    public HandbookTemplates GetHandbook()
    {
        return _eftDatabase.Handbook.Get();
    }

    public void SetHandbook(HandbookTemplates handbook)
    {
        _eftDatabase.Handbook.Set(handbook);
    }

    public JObject GetHideoutAreas()
    {
        return _eftDatabase.HideoutAreas.Get();
    }

    public void SetHideoutAreas(JObject areas)
    {
        _eftDatabase.HideoutAreas.Set(areas);
    }

    public JObject GetHideoutCustomizationOffers()
    {
        return _eftDatabase.HideoutCustomizationOffers.Get();
    }

    public void SetHideoutCustomizationOffers(JObject offers)
    {
        _eftDatabase.HideoutCustomizationOffers.Set(offers);
    }

    public JObject GetHideoutProductionRecipes()
    {
        return _eftDatabase.HideoutProductionRecipes.Get();
    }

    public void SetHideoutProductionRecipes(JObject recipes)
    {
        _eftDatabase.HideoutProductionRecipes.Set(recipes);
    }

    public JObject GetHideoutQtes()
    {
        return _eftDatabase.HideoutQtes.Get();
    }

    public void SetHideoutQtes(JObject qtes)
    {
        _eftDatabase.HideoutQtes.Set(qtes);
    }

    public JObject GetItemTemplates()
    {
        return _eftDatabase.ItemTemplates.Get();
    }

    public void SetItemTemplates(JObject templates)
    {
        _eftDatabase.ItemTemplates.Set(templates);
    }

    public JObject GetLocalWeather()
    {
        return _eftDatabase.LocalWeather.Get();
    }

    public void SetLocalWeather(JObject weather)
    {
        _eftDatabase.LocalWeather.Set(weather);
    }

    public JObject GetPrestige()
    {
        return _eftDatabase.Prestige.Get();
    }

    public void SetPrestige(JObject pretigste)
    {
        _eftDatabase.Prestige.Set(pretigste);
    }

    public JObject GetQuests()
    {
        return _eftDatabase.Quests.Get();
    }

    public void SetQuests(JObject quests)
    {
        _eftDatabase.Quests.Set(quests);
    }

    public JObject GetSettings()
    {
        return _eftDatabase.Settings.Get();
    }

    public void SetSettings(JObject settings)
    {
        _eftDatabase.Settings.Set(settings);
    }

    public JObject GetTraders()
    {
        return _eftDatabase.Traders.Get();
    }

    public void SetTraders(JObject traders)
    {
        _eftDatabase.Traders.Set(traders);
    }

    public JObject GetWeather()
    {
        return _eftDatabase.Weather.Get();
    }

    public void SetWeather(JObject weather)
    {
        _eftDatabase.Weather.Set(weather);
    }
    #endregion
}