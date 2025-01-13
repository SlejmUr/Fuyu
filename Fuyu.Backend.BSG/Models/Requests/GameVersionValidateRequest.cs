using System.Runtime.Serialization;
using Fuyu.Backend.BSG.Models.Common;

namespace Fuyu.Backend.BSG.Models.Requests;

[DataContract]
public class GameVersionValidateRequest
{
    [DataMember(Name = "version")]
    public GameVersion Version { get; set; }

    [DataMember(Name = "develop")]
    public bool develop { get; set; }
}
