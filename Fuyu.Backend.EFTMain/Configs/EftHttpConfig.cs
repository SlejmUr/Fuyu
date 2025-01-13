using System.Runtime.Serialization;
using Fuyu.Common.Config;

namespace Fuyu.Backend.EFTMain.Configs;

[DataContract]
public class EftHttpConfig : AbstractConfig
{
    [DataMember]
    public string EftServer { get; set; }

}
