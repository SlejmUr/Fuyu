using System.Runtime.Serialization;
using Fuyu.Backend.BSG.Models.Profiles.Info;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.Models.Trading;

[DataContract]
public class RagfairTraderUser : IRagfairUser
{
    public MongoId Id { get; set; }

    public EMemberCategory MemberCategory => EMemberCategory.Trader;

    public string Avatar { get; }

    public RagfairTraderUser(MongoId id)
    {
        Id = id;
    }
}