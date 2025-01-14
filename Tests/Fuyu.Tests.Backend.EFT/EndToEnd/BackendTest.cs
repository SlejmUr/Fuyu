using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Accounts;
using Fuyu.Backend.BSG.Models.Bots;
using Fuyu.Backend.BSG.Models.Raid;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.Core;
using Fuyu.Backend.Core.Models.Accounts;
using Fuyu.Backend.Core.Servers;
using Fuyu.Backend.EFTMain;
using Fuyu.Backend.EFTMain.Services;
using Fuyu.Common.Hashing;
using Fuyu.Common.IO;
using Fuyu.Common.Serialization;
using Fuyu.Tests.Backend.EFT.Networking;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccountService = Fuyu.Backend.Core.Services.AccountService;

namespace Fuyu.Tests.Backend.EFT.EndToEnd;

[TestClass]
public class BackendTest
{
    private static EftHttpClient _eftMainClient;
    private static string _eftSessionId;

    private static string CreateFuyuAccount(string username, string password)
    {
        var registerStatus = AccountService.Instance.RegisterAccount(username, password);

        if (registerStatus != ERegisterStatus.Success)
        {
            throw new Exception(registerStatus.ToString());
        }

        var hashedPassword = Sha256.Generate(password);
        var response = AccountService.Instance.LoginAccount(username, hashedPassword);

        if (response.Status != ELoginStatus.Success)
        {
            throw new Exception(response.Status.ToString());
        }

        return response.SessionId;
    }

    private static int CreateGameAccount(string sessionId, string game, string edition)
    {
        var response = AccountService.Instance.RegisterGame(sessionId, game, edition);

        if (response.Status != ERegisterStatus.Success)
        {
            throw new Exception(response.Status.ToString());
        }

        var gameAccountId = CoreOrm.Instance.GetAccount(sessionId).Games[game].Value;

        return gameAccountId;
    }

    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext testContext)
    {
        if (VFS.DirectoryExists("Fuyu/"))
        {
            Directory.Delete("Fuyu/", true);
        }

        // setup databases
        CoreLoader.Instance.Load();
        EftLoader.Instance.Load();

        // setup servers
        var coreServer = new CoreServer();
        coreServer.RegisterServices();
        coreServer.Start();

        var eftMainServer = new EftMainServer();
        eftMainServer.RegisterServices();
        eftMainServer.Start();

        // register test account
        var coreSessionId = CreateFuyuAccount("TestUser1", "TestPass1!");
        var eftAccountId = CreateGameAccount(coreSessionId, "eft", "unheard");
        _eftSessionId = Fuyu.Backend.EFTMain.Services.AccountService.Instance.LoginAccount(eftAccountId);

        // create request clients
        _eftMainClient = new EftHttpClient("http://localhost:8010", _eftSessionId, "0.16.0.2.34510");

        var account = EftOrm.Instance.GetAccount(eftAccountId);
        ProfileService.Instance.WipeProfile(account, "Usec", "5cde96047d6c8b20b577f016", "6284d6ab8e4092597733b7a7");
    }

    [TestMethod]
    public async Task TestClientAchievementList()
    {
        var response = await _eftMainClient.GetAsync("/client/achievement/list");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientAchievementStatistic()
    {
        var response = await _eftMainClient.GetAsync("/client/achievement/statistic");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientBuildList()
    {
        var response = await _eftMainClient.GetAsync("/client/builds/list");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientCheckVersion()
    {
        var response = await _eftMainClient.GetAsync("/client/checkVersion");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientCustomization()
    {
        var response = await _eftMainClient.GetAsync("/client/customization");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientCustomizationStorage()
    {
        var response = await _eftMainClient.GetAsync("/client/customization/storage");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientFriendList()
    {
        var response = await _eftMainClient.GetAsync("/client/friend/list");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientFriendRequestListInbox()
    {
        var response = await _eftMainClient.GetAsync("/client/friend/request/list/inbox");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientFriendRequestListOutbox()
    {
        var response = await _eftMainClient.GetAsync("/client/friend/request/list/outbox");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientGameBotGenerate()
    {
        // get request data
        var request = new GameBotGenerateRequest()
        {
            conditions = [
                new BotCondition()
                {
                    Role = EWildSpawnType.assault,
                    Limit = 1,
                    Difficulty = EBotDifficulty.normal
                }
            ]
        };

        // get request body
        var json = Json.Stringify(request);
        var body = Encoding.UTF8.GetBytes(json);

        // get response
        var response = await _eftMainClient.PostAsync("/client/game/bot/generate", body);
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientGameConfig()
    {
        var response = await _eftMainClient.GetAsync("/client/game/config");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientGameKeepalive()
    {
        var response = await _eftMainClient.GetAsync("/client/game/keepalive");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientLogout()
    {
        var response = await _eftMainClient.GetAsync("/client/game/logout");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientGameMode()
    {
        var request = new ClientGameModeRequest { SessionMode = ESessionMode.Pve };
        var requestJson = Json.Stringify(request);
        var requestBytes = Encoding.UTF8.GetBytes(requestJson);
        var response = await _eftMainClient.PostAsync("/client/game/mode", requestBytes);
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientGameProfileCreate()
    {
        // get request data
        var request = new GameProfileCreateRequest()
        {
            side = "Usec",
            nickname = "Senko-san",
            headId = "5cde96047d6c8b20b577f016",
            voiceId = "5fc614f40b735e7b024c76e9"
        };

        // get request body
        var json = Json.Stringify(request);
        var body = Encoding.UTF8.GetBytes(json);

        // get response
        var response = await _eftMainClient.PostAsync("/client/game/profile/create", body);
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientGameProfileList()
    {
        var response = await _eftMainClient.GetAsync("/client/game/profile/list");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientGameProfileNicknameReserved()
    {
        var response = await _eftMainClient.GetAsync("/client/game/profile/nickname/reserved");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientGameProfileNicknameValidate()
    {
        // get request data
        var request = new GameProfileNicknameValidateRequest()
        {
            Nickname = "senko"
        };

        // get request body
        var json = Json.Stringify(request);
        var body = Encoding.UTF8.GetBytes(json);

        // get response
        var response = await _eftMainClient.PostAsync("/client/game/profile/nickname/validate", body);
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientGameProfileNicknameChange()
    {
        // get request data
        var request = new GameProfileNicknameChangeRequest()
        {
            Nickname = "senko"
        };

        // get request body
        var json = Json.Stringify(request);
        var body = Encoding.UTF8.GetBytes(json);

        // get response
        var response = await _eftMainClient.PostAsync("/client/game/profile/nickname/change", body);
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientGameProfileSelect()
    {
        var request = new GameProfileSelectRequest()
        {
            Uid = ""
        };
        var json = Json.Stringify(request);
        var body = Encoding.UTF8.GetBytes(json);

        var response = await _eftMainClient.PostAsync("/client/game/profile/select", body);
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientGameStart()
    {
        var response = await _eftMainClient.GetAsync("/client/game/start");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientGameVersionValidate()
    {
        var request = new GameVersionValidateRequest()
        {

        };
        var json = Json.Stringify(request);
        var body = Encoding.UTF8.GetBytes(json);

        var response = await _eftMainClient.PostAsync("/client/game/version/validate", body);
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientGetMetricsConfig()
    {
        var response = await _eftMainClient.GetAsync("/client/getMetricsConfig");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientGlobals()
    {
        var response = await _eftMainClient.GetAsync("/client/globals");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientHandbookTemplates()
    {
        var response = await _eftMainClient.GetAsync("/client/handbook/templates");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientHideoutAreas()
    {
        var response = await _eftMainClient.GetAsync("/client/hideout/areas");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientHideoutCustomizationOfferList()
    {
        var response = await _eftMainClient.GetAsync("/client/hideout/customization/offer/list");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientProductionRecipes()
    {
        var response = await _eftMainClient.GetAsync("/client/hideout/production/recipes");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientHideoutQteList()
    {
        var response = await _eftMainClient.GetAsync("/client/hideout/qte/list");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientHideoutSettings()
    {
        var response = await _eftMainClient.GetAsync("/client/hideout/settings");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientItems()
    {
        var response = await _eftMainClient.GetAsync("/client/items");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientLanguages()
    {
        var response = await _eftMainClient.GetAsync("/client/languages");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientLocaleCh()
    {
        var response = await _eftMainClient.GetAsync("/client/locale/ch");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientLocaleCz()
    {
        var response = await _eftMainClient.GetAsync("/client/locale/cz");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientLocaleEn()
    {
        var response = await _eftMainClient.GetAsync("/client/locale/en");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientLocaleEs()
    {
        var response = await _eftMainClient.GetAsync("/client/locale/es");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientLocaleEsMx()
    {
        var response = await _eftMainClient.GetAsync("/client/locale/es-mx");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientLocaleFr()
    {
        var response = await _eftMainClient.GetAsync("/client/locale/fr");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientLocaleGe()
    {
        var response = await _eftMainClient.GetAsync("/client/locale/ge");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientLocaleHu()
    {
        var response = await _eftMainClient.GetAsync("/client/locale/hu");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientLocaleIt()
    {
        var response = await _eftMainClient.GetAsync("/client/locale/it");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientLocaleJp()
    {
        var response = await _eftMainClient.GetAsync("/client/locale/jp");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientLocaleKr()
    {
        var response = await _eftMainClient.GetAsync("/client/locale/kr");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientLocalePo()
    {
        var response = await _eftMainClient.GetAsync("/client/locale/po");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientLocalePl()
    {
        var response = await _eftMainClient.GetAsync("/client/locale/pl");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientLocaleRo()
    {
        var response = await _eftMainClient.GetAsync("/client/locale/ro");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientLocaleRu()
    {
        var response = await _eftMainClient.GetAsync("/client/locale/ru");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientLocaleSk()
    {
        var response = await _eftMainClient.GetAsync("/client/locale/sk");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientLocaleTu()
    {
        var response = await _eftMainClient.GetAsync("/client/locale/tu");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientLocalGameWeather()
    {
        var response = await _eftMainClient.GetAsync("/client/localGame/weather");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientLocations()
    {
        var response = await _eftMainClient.GetAsync("/client/locations");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientMailDialogList()
    {
        var response = await _eftMainClient.GetAsync("/client/mail/dialog/list");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientMatchGroupCurrent()
    {
        var response = await _eftMainClient.GetAsync("/client/match/group/current");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientMatchGroupExitFromMenu()
    {
        var response = await _eftMainClient.GetAsync("/client/match/group/exit_from_menu");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientMatchGroupInviteCancelAll()
    {
        var response = await _eftMainClient.GetAsync("/client/match/group/invite/cancel-all");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientMatchLocalEnd()
    {
        var account = EftOrm.Instance.GetAccount(_eftSessionId);
        var profile = EftOrm.Instance.GetProfile(account.PveId);

        // get request data
        var request = new MatchLocalEndRequest()
        {
            Results = new MatchLocalEndResult()
            {
                Profile = profile.Pmc
            }
        };

        // get request body
        var json = Json.Stringify(request);
        var body = Encoding.UTF8.GetBytes(json);

        // get response
        var response = await _eftMainClient.PostAsync("/client/match/local/end", body);
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientMatchLocalStart()
    {
        // get request data
        var request = new MatchLocalStartRequest()
        {
            location = "factory4_day",
            timeVariant = "CURR",           // CURR: left, PAST: right
            mode = "PVE",
            playerSide = "PMC"
        };

        // get request body
        var json = Json.Stringify(request);
        var body = Encoding.UTF8.GetBytes(json);

        // get response
        var response = await _eftMainClient.PostAsync("/client/match/local/start", body);
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientMenuLocaleCh()
    {
        var response = await _eftMainClient.GetAsync("/client/menu/locale/ch");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientMenuLocaleCz()
    {
        var response = await _eftMainClient.GetAsync("/client/menu/locale/cz");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientMenuLocaleEn()
    {
        var response = await _eftMainClient.GetAsync("/client/menu/locale/en");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientMenuLocaleEs()
    {
        var response = await _eftMainClient.GetAsync("/client/menu/locale/es");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientMenuLocaleEsMx()
    {
        var response = await _eftMainClient.GetAsync("/client/menu/locale/es-mx");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientMenuLocaleFr()
    {
        var response = await _eftMainClient.GetAsync("/client/menu/locale/fr");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientMenuLocaleGe()
    {
        var response = await _eftMainClient.GetAsync("/client/menu/locale/ge");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientMenuLocaleHu()
    {
        var response = await _eftMainClient.GetAsync("/client/menu/locale/hu");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientMenuLocaleIt()
    {
        var response = await _eftMainClient.GetAsync("/client/menu/locale/it");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientMenuLocaleJp()
    {
        var response = await _eftMainClient.GetAsync("/client/menu/locale/jp");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientMenuLocaleKr()
    {
        var response = await _eftMainClient.GetAsync("/client/menu/locale/kr");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientMenuLocalePo()
    {
        var response = await _eftMainClient.GetAsync("/client/menu/locale/po");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientMenuLocalePl()
    {
        var response = await _eftMainClient.GetAsync("/client/menu/locale/pl");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientMenuLocaleRo()
    {
        var response = await _eftMainClient.GetAsync("/client/menu/locale/ro");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientMenuLocaleRu()
    {
        var response = await _eftMainClient.GetAsync("/client/menu/locale/ru");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientMenuLocaleSk()
    {
        var response = await _eftMainClient.GetAsync("/client/menu/locale/sk");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientMenuLocaleTu()
    {
        var response = await _eftMainClient.GetAsync("/client/menu/locale/tu");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestNotifierChannelCreate()
    {
        var response = await _eftMainClient.GetAsync("/client/notifier/channel/create");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestProfileStatus()
    {
        var response = await _eftMainClient.GetAsync("/client/profile/status");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestPutMetrics()
    {
        var response = await _eftMainClient.GetAsync("/client/putMetrics");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientPrestigeList()
    {
        var response = await _eftMainClient.GetAsync("/client/prestige/list");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestProfileSettings()
    {
        var response = await _eftMainClient.GetAsync("/client/profile/settings");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientQuestList()
    {
        var response = await _eftMainClient.GetAsync("/client/quest/list");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientRaidConfiguration()
    {
        var response = await _eftMainClient.GetAsync("/client/raid/configuration");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientRepeatableQuestActivityPeriods()
    {
        var response = await _eftMainClient.GetAsync("/client/repeatalbeQuests/activityPeriods");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientServerList()
    {
        var response = await _eftMainClient.GetAsync("/client/server/list");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientSettings()
    {
        var response = await _eftMainClient.GetAsync("/client/settings");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientSurvey()
    {
        var response = await _eftMainClient.GetAsync("/client/survey");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientTradingApiTraderSettings()
    {
        var response = await _eftMainClient.GetAsync("/client/trading/api/traderSettings");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestClientWeather()
    {
        var response = await _eftMainClient.GetAsync("/client/weather");
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestEquipmentBuildSaving()
    {
        // get request data
        var request = new EquipmentBuildSaveRequest()
        {
            Id = new MongoId(true),
            Name = "TestEquipmentBuild",
            Items = [],
            Root = new MongoId(true)
        };

        // get request body
        var json = Json.Stringify(request);
        var body = Encoding.UTF8.GetBytes(json);

        // get response
        var response = await _eftMainClient.PostAsync("/client/builds/equipment/save", body);
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestWeaponBuildSaving()
    {
        // get request data
        var request = new WeaponBuildSaveRequest()
        {
            Id = new MongoId(true),
            Name = "TestEquipmentBuild",
            Items = [],
            Root = new MongoId(true)
        };

        // get request body
        var json = Json.Stringify(request);
        var body = Encoding.UTF8.GetBytes(json);

        // get response
        var response = await _eftMainClient.PostAsync("/client/builds/weapon/save", body);
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestMagazineBuildSaving()
    {
        // get request data
        var request = new MagazineBuildSaveRequest()
        {
            Id = new MongoId(true),
            Name = "TestEquipmentBuild",
            Items = [],
            BottomCount = 0,
            TopCount = 0,
            Caliber = "Caliber9x39"
        };

        // get request body
        var json = Json.Stringify(request);
        var body = Encoding.UTF8.GetBytes(json);

        // get response
        var response = await _eftMainClient.PostAsync("/client/builds/weapon/save", body);
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestGetOtherProfile()
    {
        var profile = EftOrm.Instance.GetActiveProfile(_eftSessionId);

        // get request data
        var request = new GetOtherProfileRequest()
        {
            AccountId = profile.Pmc.aid
        };

        // get request body
        var json = Json.Stringify(request);
        var body = Encoding.UTF8.GetBytes(json);

        // get response
        var response = await _eftMainClient.PostAsync("/client/profile/view", body);
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public async Task TestSearchOtherProfile()
    {
        var profile = EftOrm.Instance.GetActiveProfile(_eftSessionId);

        // get request data
        var request = new SearchOtherProfileRequest()
        {
            Nickname = "senko"
        };

        // get request body
        var json = Json.Stringify(request);
        var body = Encoding.UTF8.GetBytes(json);

        // get response
        var response = await _eftMainClient.PostAsync("/client/game/profile/search", body);
        var result = Encoding.UTF8.GetString(response.Body);

        Assert.IsFalse(string.IsNullOrEmpty(result));
    }
}