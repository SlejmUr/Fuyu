using System.Runtime.Serialization;
using Fuyu.Backend.BSG.Models.Profiles.Info;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.Models.Trading;

public interface IRagfairUser
{
    [DataMember(Name = "id")]
    public MongoId Id { get; set; }

    [DataMember(Name = "memberType")]
    public EMemberCategory MemberCategory { get; }
}