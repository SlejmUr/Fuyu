using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Fuyu.Backend.BSG.Models.Trading;

[DataContract]
public class HandbookTemplates
{
    [DataMember(Name = "Categories")]
    public List<HandbookCategory> Categories { get; set; }

    [DataMember(Name = "Items")]
    public List<HandbookItem> Items { get; set; }
}