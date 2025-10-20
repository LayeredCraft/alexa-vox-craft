using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace AlexaVoxCraft.MediatR.DI;

/// <summary>
/// Configuration class for Alexa skill services that defines skill-specific settings
/// and controls the registration of request handlers and other skill components.
/// </summary>
public class SkillServiceConfiguration
{
    /// <summary>
    /// The configuration section name used for binding this configuration from appsettings.json.
    /// </summary>
    public const string SectionName = "SkillConfiguration";
    
    /// <summary>
    /// Gets or sets a custom user agent string to include in HTTP requests made by the skill.
    /// </summary>
    public string? CustomUserAgent { get; set; }
    
    /// <summary>
    /// Gets or sets the Alexa skill ID used for request validation.
    /// This should match the skill ID configured in the Alexa Developer Console.
    /// </summary>
    public string? SkillId { get; set; }

    /// <summary>
    /// Gets or sets the service lifetime for registered handlers and services.
    /// </summary>
    /// <value>The service lifetime. Defaults to <see cref="ServiceLifetime.Transient"/>.</value>
    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;

    /// <summary>
    /// Gets or sets the timeout buffer in milliseconds for cancellation token handling in Lambda functions.
    /// This buffer is subtracted from the Lambda's remaining execution time to allow for
    /// graceful shutdown and telemetry flushing before the Lambda times out.
    /// </summary>
    /// <value>The timeout buffer in milliseconds. Defaults to 250.</value>
    /// <remarks>
    /// When a Lambda function approaches its timeout limit, this buffer ensures that:
    /// <list type="bullet">
    /// <item><description>OpenTelemetry spans and metrics can be properly flushed</description></item>
    /// <item><description>Logging can complete gracefully</description></item>
    /// <item><description>Resources can be properly disposed</description></item>
    /// <item><description>The function doesn't get forcefully terminated mid-operation</description></item>
    /// </list>
    /// A smaller buffer reduces the safety margin but maximizes execution time.
    /// A larger buffer provides more safety but reduces available execution time.
    /// </remarks>
    public int CancellationTimeoutBufferMilliseconds { get; set; } = 250;
    
    /// <summary>
    /// Gets the list of assemblies to scan for request handlers and other skill components.
    /// </summary>
    internal List<Assembly> AssembliesToRegister { get; } = new();

    /// <summary>
    /// Registers request handlers and services from the assembly containing the specified type.
    /// </summary>
    /// <typeparam name="T">A type whose assembly should be scanned for services.</typeparam>
    /// <returns>This configuration instance for method chaining.</returns>
    public SkillServiceConfiguration RegisterServicesFromAssemblyContaining<T>()
        => RegisterServicesFromAssemblyContaining(typeof(T));
    
    /// <summary>
    /// Registers request handlers and services from the assembly containing the specified type.
    /// </summary>
    /// <param name="type">A type whose assembly should be scanned for services.</param>
    /// <returns>This configuration instance for method chaining.</returns>
    public SkillServiceConfiguration RegisterServicesFromAssemblyContaining(Type type)
        => RegisterServicesFromAssembly(type.Assembly);
    
    /// <summary>
    /// Registers request handlers and services from the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly to scan for services.</param>
    /// <returns>This configuration instance for method chaining.</returns>
    public SkillServiceConfiguration RegisterServicesFromAssembly(Assembly assembly)
    {
        AssembliesToRegister.Add(assembly);

        return this;
    }
    
    /// <summary>
    /// Registers request handlers and services from multiple assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies to scan for services.</param>
    /// <returns>This configuration instance for method chaining.</returns>
    public SkillServiceConfiguration RegisterServicesFromAssemblies(
        params Assembly[] assemblies)
    {
        AssembliesToRegister.AddRange(assemblies);

        return this;
    }

}