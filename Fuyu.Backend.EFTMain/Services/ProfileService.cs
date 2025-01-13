using System;
using System.Linq;
using Fuyu.Backend.BSG.Models.Accounts;
using Fuyu.Backend.BSG.Models.Profiles;
using Fuyu.Backend.BSG.Models.Profiles.Info;
using Fuyu.Backend.EFTMain.Configs;
using Fuyu.Common.Config;
using Fuyu.Common.Hashing;
using Fuyu.Common.IO;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.EFTMain.Services;

public class ProfileService
{
    public static ProfileService Instance => instance.Value;
    private static readonly Lazy<ProfileService> instance = new(() => new ProfileService());

    private readonly EftOrm _eftOrm;

    /// <summary>
    /// The construction of this class is handled in the <see cref="instance"/> (<see cref="Lazy{T}"/>)
    /// </summary>
    private ProfileService()
    {
        _eftOrm = EftOrm.Instance;
    }

    public string CreateProfile(int accountId)
    {
        var builds = _eftOrm.GetDefaultBuilds();
        var profile = new EftProfile()
        {
            Pmc = new Profile(),
            Savage = new Profile(),
            Customization = [],
            Builds = builds,
            ShouldWipe = true
        };

        // generate new ids
        var pmcId = new MongoId(true).ToString();
        var savageId = new MongoId(pmcId, 1, false).ToString();

        // set profile info
        profile.Pmc._id = pmcId;
        profile.Pmc.aid = accountId;

        profile.Savage._id = savageId;
        profile.Savage.aid = accountId;

        // store profile
        _eftOrm.SetOrAddProfile(profile);
        WriteToDisk(profile);

        return pmcId;
    }

    public string WipeProfile(EftAccount account, string side, string headId, string voiceId)
    {
        var profile = _eftOrm.GetActiveProfile(account);
        var pmcId = profile.Pmc._id;
        var savageId = profile.Savage._id;

        // create profiles
        var edition = _eftOrm.GetWipeProfile(account.Edition);

        profile.Savage = edition[EPlayerSide.Savage].Clone();

        // NOTE: Case-sensitive
        // -- seionmoya, 2024-10-13
        switch (side)
        {
            case "Bear":
                profile.Pmc = edition[EPlayerSide.Bear];
                break;

            case "Usec":
                profile.Pmc = edition[EPlayerSide.Usec];
                break;

            default:
                throw new Exception("Unsupported faction");
        }

        // setup savage
        profile.Savage._id = savageId;
        profile.Savage.aid = account.Id;

        // setup pmc
        var voiceTemplate = _eftOrm.GetCustomization(voiceId);

        profile.Pmc._id = pmcId;
        profile.Pmc.savage = savageId;
        profile.Pmc.aid = account.Id;
        profile.Pmc.Info.Nickname = account.Username;
        profile.Pmc.Info.LowerNickname = account.Username.ToLowerInvariant();
        profile.Pmc.Info.Voice = voiceTemplate._name;
        profile.Pmc.Customization.Head = headId;

        // wipe done
        profile.ShouldWipe = false;

        // store profile
        _eftOrm.SetOrAddProfile(profile);
        WriteToDisk(profile);

        return profile.Pmc._id;
    }

    /// <summary>
    /// Checks whether a nickname is valid for the client
    /// </summary>
    /// <param name="nickname">The new nickname</param>
    /// <param name="status">The status returned to the client</param>
    /// <returns>True if the nickname is valid</returns>
    public ENicknameChangeResult IsValidNickname(string nickname)
    {
        var service = ConfigService.GetInstance("eft-main");
        var nicknameConfig = service.GetOrCreate<NicknameConfig>("nickname_config");

        //TODO: Handle all results
        // We dont handle NicknameTaken
        if (nickname.Length < 3)
        {
            return ENicknameChangeResult.TooShort;
        }

        if (nickname.Length > 15)
        {
            return ENicknameChangeResult.CharacterLimit;
        }

        // select characters that has digits and checks if more than 4
        if (nickname.Select(char.IsDigit).Count() > 4 )
        {
            return ENicknameChangeResult.DigitsLimit;
        }

        // checks if any of characters contains invalid symbol
        if (nickname.Any(nicknameConfig.NicknameInvalidSymbols.Contains))
        {
            return ENicknameChangeResult.WrongSymbol;
        }

        return ENicknameChangeResult.Ok;
    }

    public void WriteToDisk(EftProfile profile)
    {
        VFS.WriteTextFile(
            $"./Fuyu/Profiles/EFT/{profile.Pmc._id}.json",
            Json.Stringify(profile));
    }
}