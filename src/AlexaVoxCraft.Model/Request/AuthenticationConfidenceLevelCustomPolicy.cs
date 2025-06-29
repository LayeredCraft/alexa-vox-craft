using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request;

public class AuthenticationConfidenceLevelCustomPolicy
{
    public AuthenticationConfidenceLevelCustomPolicy(){}

    public AuthenticationConfidenceLevelCustomPolicy(string policyName)
    {
        PolicyName = policyName;
    }

    [JsonPropertyName("policyName")]
    public string PolicyName { get; set; }
}