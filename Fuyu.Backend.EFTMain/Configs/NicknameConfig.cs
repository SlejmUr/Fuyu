using System.Collections.Generic;
using System.Runtime.Serialization;
using Fuyu.Common.Config;

namespace Fuyu.Backend.EFTMain.Configs;

[DataContract]
public class NicknameConfig : AbstractConfig
{
    [DataMember]
    public List<char> NicknameInvalidSymbols { get; set; } = ['$', '&', '@', '\\', '/', '"'];

    [DataMember]
    public int NicknameChangeTimeoutIntervalSeconds { get; set; } = 6000;
}
