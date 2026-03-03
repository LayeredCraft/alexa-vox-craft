using System.Collections.Concurrent;
using System.Text.Json;
using AlexaVoxCraft.Model.Response.Directive;

namespace AlexaVoxCraft.Model.Response.Converters;

public class DirectiveConverter : BasePolymorphicConverter<IDirective>
{
    private static readonly ConcurrentDictionary<string, Type> _directiveDerivedTypes = new(
        new Dictionary<string, Type>
        {
            { AudioPlayerPlayDirective.DirectiveType, typeof(AudioPlayerPlayDirective) },
            { ClearQueueDirective.DirectiveType, typeof(ClearQueueDirective) },
            { DialogConfirmIntent.DirectiveType, typeof(DialogConfirmIntent) },
            { DialogConfirmSlot.DirectiveType, typeof(DialogConfirmSlot) },
            { DialogDelegate.DirectiveType, typeof(DialogDelegate) },
            { DialogElicitSlot.DirectiveType, typeof(DialogElicitSlot) },
            { HintDirective.DirectiveType, typeof(HintDirective) },
            { StopDirective.DirectiveType, typeof(StopDirective) },
            { VideoAppDirective.DirectiveType, typeof(VideoAppDirective) },
            { StartConnectionDirective.DirectiveType, typeof(StartConnectionDirective) },
            { CompleteTaskDirective.DirectiveType, typeof(CompleteTaskDirective) },
            { DialogUpdateDynamicEntities.DirectiveType, typeof(DialogUpdateDynamicEntities) }
        });

    private static readonly ConcurrentDictionary<string, Func<JsonElement, Type>> _directiveDataDrivenTypeFactories = new(
        new Dictionary<string, Func<JsonElement, Type>>
        {
            { ConnectionSendRequest.DirectiveType, ConnectionSendRequestFactory.Create }
        });

    /// <summary>
    /// Registers a derived <see cref="IDirective"/> type for deserialization using a static type discriminator key.
    /// </summary>
    /// <typeparam name="TDirective">The directive type to register.</typeparam>
    /// <param name="key">The value of the <c>type</c> property that identifies this directive in JSON.</param>
    public static void RegisterDirectiveDerivedType<TDirective>(string key) where TDirective : IDirective
    {
        _directiveDerivedTypes.TryAdd(key, typeof(TDirective));
    }

    /// <summary>
    /// Registers a data-driven type factory for directives whose concrete type depends on additional JSON properties beyond the type discriminator.
    /// </summary>
    /// <param name="key">The value of the <c>type</c> property that triggers this factory.</param>
    /// <param name="factory">A function that inspects the full JSON element and returns the appropriate <see cref="Type"/>.</param>
    public static void RegisterDirectiveDataDrivenTypeFactory(string key, Func<JsonElement, Type> factory)
    {
        _directiveDataDrivenTypeFactories.TryAdd(key, factory);
    }

    protected override string TypeDiscriminatorPropertyName => "type";

    protected override IDictionary<string, Type> DerivedTypes => _directiveDerivedTypes;

    protected override IDictionary<string, Func<JsonElement, Type>> DataDrivenTypeFactories => _directiveDataDrivenTypeFactories;


    protected override Func<JsonElement, Type?>? CustomTypeResolver => null;
    protected override Type? DefaultType => typeof(JsonDirective);
}