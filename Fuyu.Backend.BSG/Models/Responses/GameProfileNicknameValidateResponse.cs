using System.Runtime.Serialization;
using Fuyu.Backend.BSG.Models.Profiles;

namespace Fuyu.Backend.BSG.Models.Responses;

[DataContract]
public class GameProfileNicknameValidateResponse
{
    [DataMember]
    public ENicknameChangeResult status { get; set; }
}