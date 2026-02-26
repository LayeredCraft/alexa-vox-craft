using System.Text.Json.Serialization;

namespace AlexaVoxCraft.InSkillPurchasing.Models;

/// <summary>
/// Represents the voice purchasing enabled state for the current skill.
/// </summary>
public sealed record PurchasingEnabled([property: JsonPropertyName("enabled")] bool Enabled);