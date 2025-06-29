using System.Text.Json;
using AlexaVoxCraft.Model.Response.Directive;

namespace AlexaVoxCraft.Model.Response.Converters;

public class DirectiveConverter : BasePolymorphicConverter<IDirective>
{
    private static IDictionary<string, Type> _directiveDerivedTypes = new Dictionary<string, Type>
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
    };

    private static Dictionary<string, Func<JsonElement, Type>> _directiveDataDrivenTypeFactories =
        new()
        {
            { ConnectionSendRequest.DirectiveType, ConnectionSendRequestFactory.Create }
        };

    public static void RegisterDirectiveDerivedType<TDirective>(string key) where TDirective : IDirective
    {
        _directiveDerivedTypes.TryAdd(key, typeof(TDirective));
    }
    
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