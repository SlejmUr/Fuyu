using System.Runtime.Serialization;

namespace Fuyu.Backend.BSG.Models.Common;

[DataContract]
public class GameVersion
{
    [DataMember(Name = "major")]
    public string Major;

    [DataMember(Name = "minor")]
    public string Minor;

    [DataMember(Name = "game")]
    public string Game;

    [DataMember(Name = "backend")]
    public string Backend;

    [DataMember(Name = "taxonomy")]
    public string Taxonomy;
}
