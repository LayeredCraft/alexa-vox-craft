using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.DataSources;

public class ListPage
{
    public ListPage()
    {
        ListItems = new List<object>();
    }

    [JsonPropertyName("listItems")]
    public List<object> ListItems { get; set; }
}