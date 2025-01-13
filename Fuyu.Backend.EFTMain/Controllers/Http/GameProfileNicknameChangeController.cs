using System;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Profiles;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain;
using Fuyu.Backend.EFTMain.Configs;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Services;
using Fuyu.Common.Config;

namespace Fuyu.Backend.EFT.Controllers.Http;

public class GameProfileNicknameChangeController : AbstractEftHttpController<GameProfileNicknameChangeRequest>
{
    private readonly ProfileService _profileService;
    private readonly EftOrm _eftOrm;

    public GameProfileNicknameChangeController() : base("/client/game/profile/nickname/change")
    {
        _profileService = ProfileService.Instance;
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(EftHttpContext context, GameProfileNicknameChangeRequest request)
    {
        var service = ConfigService.GetInstance("eft-main");
        var nicknameConfig = service.GetOrCreate<NicknameConfig>("nickname_config");
        var result = _profileService.IsValidNickname(request.Nickname);

        if (result == ENicknameChangeResult.Ok)
        {
            //TODO: Save profile properly, currently doesn't persist?
            var profile = _eftOrm.GetActiveProfile(context.SessionId);

            // checks for date larger than second.
            if (DateTimeOffset.Now - DateTimeOffset.FromUnixTimeSeconds(profile.Pmc.Info.NicknameChangeDate) 
                > TimeSpan.FromSeconds(nicknameConfig.NicknameChangeTimeoutIntervalSeconds))
            {
                result = ENicknameChangeResult.NicknameChangeTimeout;
                goto ErrorMessageSet;
            }

            profile.Pmc.Info.Nickname = request.Nickname;
            profile.Pmc.Info.LowerNickname = request.Nickname.ToLower();
            profile.Pmc.Info.NicknameChangeDate = DateTimeOffset.Now.ToUnixTimeSeconds();

            _profileService.WriteToDisk(profile);
        }
    ErrorMessageSet:
        // TODO: Find if there is a more proper usage of EBackendErrorCode for this switch (find actual error message in globals.json)
        // Eror messages are inside the locale json. prefixed with ENicknameError/
        var errorMessage = result switch
        {
            ENicknameChangeResult.WrongSymbol => EBackendErrorCode.NicknameNotValid,
            ENicknameChangeResult.TooShort => EBackendErrorCode.NicknameNotValid,
            ENicknameChangeResult.CharacterLimit => EBackendErrorCode.NicknameNotValid,
            ENicknameChangeResult.InvalidNickname => EBackendErrorCode.NicknameNotValid,
            ENicknameChangeResult.NicknameTaken => EBackendErrorCode.NicknameNotUnique,
            ENicknameChangeResult.NicknameChangeTimeout => EBackendErrorCode.NicknameChangeTimeout,
            ENicknameChangeResult.DigitsLimit => EBackendErrorCode.NicknameNotValid,
            ENicknameChangeResult.Ok => EBackendErrorCode.None,
            _ => EBackendErrorCode.None,
        };

        var response = new ResponseBody<GameProfileNicknameChangeResponse>()
        {
            err = (int)errorMessage,
            errmsg = errorMessage != EBackendErrorCode.None ? errorMessage.ToString() : null,
            data = new GameProfileNicknameChangeResponse()
            {
                Status = result
            }
        };

        return context.SendResponseAsync(response, true, true);
    }
}