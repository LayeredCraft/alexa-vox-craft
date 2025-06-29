using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Request;

namespace AlexaVoxCraft.Model.Apl;

public class APLSkillRequest : SkillRequest
{
    private APLContext _context;

    [JsonPropertyName("context")]
    public new APLContext Context
    {
        get => _context;
        set
        {
            base.Context = value;
            _context = value;
        }
    }
}