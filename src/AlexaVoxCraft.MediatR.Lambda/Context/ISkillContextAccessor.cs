namespace AlexaVoxCraft.MediatR.Lambda.Context;

/// <summary>
/// Provides access to the current skill context within the request processing pipeline.
/// This interface allows components to access request-scoped data and utilities
/// without explicitly passing the context through method parameters.
/// </summary>
public interface ISkillContextAccessor
{
    /// <summary>
    /// Gets or sets the current skill context for the request being processed.
    /// Returns null if no context is currently set or if called outside of request processing.
    /// </summary>
    /// <value>The current skill context, or null if no context is available.</value>
    SkillContext? SkillContext { get; set; }
}