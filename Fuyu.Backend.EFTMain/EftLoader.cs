using System;
using System.Collections.Generic;
using Fuyu.Backend.BSG.Models.Accounts;
using Fuyu.Backend.BSG.Models.Customization;
using Fuyu.Backend.BSG.Models.Locations;
using Fuyu.Backend.BSG.Models.Profiles;
using Fuyu.Backend.BSG.Models.Profiles.Info;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Common.Delegates;
using Fuyu.Common.IO;
using Fuyu.Common.Serialization;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.EFTMain;

public class EftLoader
{
    public static EftLoader Instance => instance.Value;
    private static readonly Lazy<EftLoader> instance = new(() => new EftLoader());

    private readonly EftOrm _eftOrm;

    public LoadCallback OnLoadAccounts;
    public LoadCallback OnLoadProfiles;
    public LoadCallback OnLoadSessions;
    public LoadCallback OnLoadLanguages;
    public LoadCallback OnLoadGlobalLocales;
    public LoadCallback OnLoadMenuLocales;
    public LoadCallback OnLoadCustomizations;
    public LoadCallback OnLoadCustomizationStorage;
    public LoadCallback OnLoadDefaultBuilds;
    public LoadCallback OnLoadWipeProfiles;
    public LoadCallback OnLoadWorldMap;
    public LoadCallback OnLoadHideoutSettings;
    public LoadCallback OnLoadAchievementStatistics;
    public LoadCallback OnLoadAchievements;
    public LoadCallback OnLoadGlobals;
    public LoadCallback OnLoadHandbook;
    public LoadCallback OnLoadHideoutAreas;
    public LoadCallback OnLoadHideoutCustomizationOffers;
    public LoadCallback OnLoadHideoutProductionRecipes;
    public LoadCallback OnLoadHideoutQtes;
    public LoadCallback OnLoadItemTemplates;
    public LoadCallback OnLoadLocalWeather;
    public LoadCallback OnLoadPretigste;
    public LoadCallback OnLoadQuests;
    public LoadCallback OnLoadSettings;
    public LoadCallback OnLoadTraders;
    public LoadCallback OnLoadWeather;

    /// <summary>
    /// The construction of this class is handled in the <see cref="instance"/> (<see cref="Lazy{T}"/>)
    /// </summary>
    private EftLoader()
    {
        _eftOrm = EftOrm.Instance;

        OnLoadAccounts += LoadAccounts;
        OnLoadProfiles += LoadProfiles;
        OnLoadSessions += LoadSessions;
        OnLoadLanguages += LoadLanguages;
        OnLoadGlobalLocales += LoadGlobalLocales;
        OnLoadMenuLocales += LoadMenuLocales;
        OnLoadCustomizations += LoadCustomizations;
        OnLoadCustomizationStorage += LoadCustomizationStorage;
        OnLoadDefaultBuilds += LoadDefaultBuilds;
        OnLoadWipeProfiles += LoadWipeProfiles;
        OnLoadWorldMap += LoadWorldMap;
        OnLoadHideoutSettings += LoadHideoutSettings;
        OnLoadAchievementStatistics += LoadAchievementStatistics;
        OnLoadAchievements += LoadAchievements;
        OnLoadGlobals += LoadGlobals;
        OnLoadHandbook += LoadHandbook;
        OnLoadHideoutAreas += LoadHideoutAreas;
        OnLoadHideoutCustomizationOffers += LoadHideoutCustomizationOffers;
        OnLoadHideoutProductionRecipes += LoadHideoutProductionRecipes;
        OnLoadHideoutQtes += LoadHideoutQtes;
        OnLoadItemTemplates += LoadItemTemplates;
        OnLoadLocalWeather += LoadLocalWeather;
        OnLoadPretigste += LoadPretigste;
        OnLoadQuests += LoadQuests;
        OnLoadSettings += LoadSettings;
        OnLoadTraders += LoadTraders;
        OnLoadWeather += LoadWeather;
    }

    // NOTE: load order is VERY important!
    // -- seionmoya, 2024/09/04
    public void Load()
    {
        // set data source
        Resx.SetSource("eft", typeof(EftDatabase).Assembly);

        // load accounts
        LoadAccounts();
        LoadProfiles();
        LoadSessions();

        // load locales
        LoadLanguages();
        LoadGlobalLocales();
        LoadMenuLocales();

        // load templates
        LoadCustomizations();
        LoadCustomizationStorage();
        LoadDefaultBuilds();
        LoadWipeProfiles();
        LoadWorldMap();
        LoadHideoutSettings();
        LoadAchievementStatistics();

        // JOBJECT
        LoadAchievements();
        LoadGlobals();
        LoadHandbook();
        LoadHideoutAreas();
        LoadHideoutCustomizationOffers();
        LoadHideoutProductionRecipes();
        LoadHideoutQtes();
        LoadItemTemplates();
        LoadLocalWeather();
        LoadPretigste();
        LoadQuests();
        LoadSettings();
        LoadTraders();
        LoadWeather();
    }

    private void LoadAccounts()
    {
        var path = "./Fuyu/Accounts/EFT/";

        if (!VFS.DirectoryExists(path))
        {
            VFS.CreateDirectory(path);
        }

        var files = VFS.GetFiles(path);

        foreach (var filepath in files)
        {
            var json = VFS.ReadTextFile(filepath);
            var account = Json.Parse<EftAccount>(json);
            _eftOrm.SetOrAddAccount(account);

            Terminal.WriteLine($"Loaded EFT account {account.Id}");
        }
    }

    private void LoadProfiles()
    {
        var path = "./Fuyu/Profiles/EFT/";

        if (!VFS.DirectoryExists(path))
        {
            VFS.CreateDirectory(path);
        }

        var files = VFS.GetFiles(path);

        foreach (var filepath in files)
        {
            var json = VFS.ReadTextFile(filepath);
            var profile = Json.Parse<EftProfile>(json);
            _eftOrm.SetOrAddProfile(profile);

            Terminal.WriteLine($"Loaded EFT profile {profile.Pmc._id}");
        }
    }

    private void LoadSessions()
    {
        // intentionally empty
        // sessions are created when users are logged in successfully
        // -- seionmoya, 2024/09/06
    }

    private void LoadCustomizations()
    {
        var json = Resx.GetText("eft", "database.client.customization.json");
        var response = Json.Parse<ResponseBody<Dictionary<string, CustomizationTemplate>>>(json);

        foreach (var kvp in response.data)
        {
            _eftOrm.SetOrAddCustomization(kvp.Key, kvp.Value);
        }
    }

    private void LoadCustomizationStorage()
    {
        var json = Resx.GetText("eft", "database.client.customization.storage.json");
        var response = Json.Parse<ResponseBody<CustomizationStorageEntry[]>>(json);

        foreach (var entry in response.data)
        {
            _eftOrm.SetOrAddCustomizationStorage(entry);
        }
    }

    private void LoadLanguages()
    {
        var json = Resx.GetText("eft", $"database.locales.client.languages.json");
        var response = Json.Parse<ResponseBody<Dictionary<string, string>>>(json);

        foreach (var kvp in response.data)
        {
            _eftOrm.SetOrAddLanguage(kvp.Key, kvp.Value);
        }
    }

    private void LoadGlobalLocales()
    {
        var languages = _eftOrm.GetLanguages();

        foreach (var languageId in languages.Keys)
        {
            var json = Resx.GetText("eft", $"database.locales.client.locale-{languageId}.json");
            var response = Json.Parse<ResponseBody<Dictionary<string, string>>>(json);

            _eftOrm.SetOrAddGlobalLocale(languageId, response.data);
        }
    }

    private void LoadMenuLocales()
    {
        var languages = _eftOrm.GetLanguages();

        foreach (var languageId in languages.Keys)
        {
            var json = Resx.GetText("eft", $"database.locales.client.menu.locale-{languageId}.json");
            var response = Json.Parse<ResponseBody<MenuLocaleResponse>>(json);

            _eftOrm.SetOrAddMenuLocale(languageId, response.data);
        }
    }

    private void LoadDefaultBuilds()
    {
        var json = Resx.GetText("eft", "database.client.builds.list.json");
        var response = Json.Parse<ResponseBody<BuildsListResponse>>(json);
        _eftOrm.SetDefaultBuilds(response.data);
    }

    private void LoadWipeProfiles()
    {
        // profile
        var bearJson = Resx.GetText("eft", "database.profiles.player.unheard-bear.json");
        var usecJson = Resx.GetText("eft", "database.profiles.player.unheard-usec.json");
        var savageJson = Resx.GetText("eft", "database.profiles.player.savage.json");

        _eftOrm.SetOrAddWipeProfile("unheard", new Dictionary<EPlayerSide, Profile>()
        {
            { EPlayerSide.Bear, Json.Parse<Profile>(bearJson) },
            { EPlayerSide.Usec, Json.Parse<Profile>(usecJson) },
            { EPlayerSide.Savage, Json.Parse<Profile>(savageJson) }
        });
    }

    private void LoadAchievementStatistics()
    {
        var json = Resx.GetText("eft", "database.client.achievement.statistic.json");
        var statistics = Json.Parse<AchievementStatisticResponse>(json);
        _eftOrm.SetAchievementStatistics(statistics);
    }

    // TODO: parse from model
    // -- seionmoya, 2024-01-09
    private void LoadWorldMap()
    {
        var json = Resx.GetText("eft", "database.client.locations.json");
        //var worldmap = Json.Parse<WorldMap>(json);
        var worldmap = JObject.Parse(json);
        _eftOrm.SetWorldMap(worldmap);
    }

    private void LoadHideoutSettings()
    {
        var json = Resx.GetText("eft", "database.client.hideout.settings.json");
        var settings = Json.Parse<HideoutSettingsResponse>(json);
        _eftOrm.SetHideoutSettings(settings);
    }

    private void LoadAchievements()
    {
        var json = Resx.GetText("eft", "database.client.achievement.list.json");
        var achievements = JObject.Parse(json);
        _eftOrm.SetAchievements(achievements);
    }

    private void LoadGlobals()
    {
        var json = Resx.GetText("eft", "database.client.globals.json");
        var globals = JObject.Parse(json);
        _eftOrm.SetGlobals(globals);
    }

    private void LoadHandbook()
    {
        var json = Resx.GetText("eft", "database.client.handbook.templates.json");
        var handbook = Json.Parse<HandbookTemplates>(json);
        _eftOrm.SetHandbook(handbook);
    }

    private void LoadHideoutAreas()
    {
        var json = Resx.GetText("eft", "database.client.hideout.areas.json");
        var areas = JObject.Parse(json);
        _eftOrm.SetHideoutAreas(areas);
    }

    private void LoadHideoutCustomizationOffers()
    {
        var json = Resx.GetText("eft", "database.client.hideout.customization.offer.list.json");
        var offers = JObject.Parse(json);
        _eftOrm.SetHideoutCustomizationOffers(offers);
    }

    private void LoadHideoutProductionRecipes()
    {
        var json = Resx.GetText("eft", "database.client.hideout.production.recipes.json");
        var recipes = JObject.Parse(json);
        _eftOrm.SetHideoutProductionRecipes(recipes);
    }

    private void LoadHideoutQtes()
    {
        var json = Resx.GetText("eft", "database.client.hideout.qte.list.json");
        var qtes = JObject.Parse(json);
        _eftOrm.SetHideoutQtes(qtes);
    }

    private void LoadItemTemplates()
    {
        var json = Resx.GetText("eft", "database.client.items.json");
        var items = JObject.Parse(json);
        _eftOrm.SetItemTemplates(items);
    }

    private void LoadLocalWeather()
    {
        var json = Resx.GetText("eft", "database.client.localGame.weather.json");
        var weather = JObject.Parse(json);
        _eftOrm.SetLocalWeather(weather);
    }

    private void LoadPretigste()
    {
        var json = Resx.GetText("eft", "database.client.prestige.list.json");
        var prestige = JObject.Parse(json);
        _eftOrm.SetPrestige(prestige);
    }

    private void LoadQuests()
    {
        var json = Resx.GetText("eft", "database.client.quest.list.json");
        var quests = JObject.Parse(json);
        _eftOrm.SetQuests(quests);
    }

    private void LoadSettings()
    {
        var json = Resx.GetText("eft", "database.client.settings.json");
        var settings = JObject.Parse(json);
        _eftOrm.SetSettings(settings);
    }

    private void LoadTraders()
    {
        var json = Resx.GetText("eft", "database.client.trading.api.traderSettings.json");
        var traders = JObject.Parse(json);
        _eftOrm.SetTraders(traders);
    }

    private void LoadWeather()
    {
        var json = Resx.GetText("eft", "database.client.weather.json");
        var weather = JObject.Parse(json);
        _eftOrm.SetWeather(weather);
    }
}