namespace AlexaVoxCraft.MediatR.Annotations;

/// <summary>
/// Sort order for pipeline behaviors. Lower numbers run earlier.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class PipelineOrderAttribute : Attribute
{
    /// <summary>
    /// The sort order (ascending). Defaults to 0.
    /// </summary>
    public int Order { get; }

    /// <summary>
    /// Creates a new pipeline order attribute.
    /// </summary>
    /// <param name="order">Lower numbers run earlier.</param>
    public PipelineOrderAttribute(int order) => Order = order;
}