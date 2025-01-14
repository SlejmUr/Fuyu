using System.Runtime.Serialization;
using Fuyu.Backend.BSG.Models.Profiles;
using Fuyu.Backend.BSG.Models.Profiles.Info;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.Models.Trading;

[DataContract]
public class RagfairPlayerUser : IRagfairUser
{
    public MongoId Id { get; set; }

    [DataMember(Name = "aid")]
    public int Aid { get; set; }

    public EMemberCategory MemberCategory { get; set; }

    [DataMember(Name = "selectedMemberCategory")]
    public EMemberCategory SelectedMemberCategory { get; set; }

    [DataMember(Name = "nickname")]
    public string Nickname { get; set; }

    [DataMember(Name = "rating")]
    public float Rating { get; set; }

    [DataMember(Name = "isRatingGrowing")]
    public bool IsRatingGrowing { get; set; }

    public RagfairPlayerUser(Profile profile) : this(profile._id, profile.aid,
        profile.Info.MemberCategory, profile.Info.SelectedMemberCategory,
        profile.Info.Nickname, profile.RagfairInfo.rating, profile.RagfairInfo.isRatingGrowing)
    {
    }

    public RagfairPlayerUser(MongoId id, int aid, EMemberCategory category, EMemberCategory selectedMemberCategory,
        string nickname, float rating, bool isRatingGrowing)
    {
        Id = id;
        Aid = aid;
        MemberCategory = category;
        SelectedMemberCategory = selectedMemberCategory;
        Nickname = nickname;
        Rating = rating;
        IsRatingGrowing = isRatingGrowing;
    }
}