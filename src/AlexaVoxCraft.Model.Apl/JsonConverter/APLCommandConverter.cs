using System;
using System.Collections.Generic;
using AlexaVoxCraft.Model.Apl.Commands;
using AlexaVoxCraft.Model.Apl.Extensions.Backstack;
using AlexaVoxCraft.Model.Apl.Extensions.DataStore;
using AlexaVoxCraft.Model.Apl.Extensions.SmartMotion;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class APLCommandConverter : BasePolymorphicConverter<APLCommand>
{
    private static Dictionary<string, Type> _derivedTypes = new()
    {
        [nameof(Idle)] = typeof(Idle),
        [nameof(Sequential)] = typeof(Sequential),
        [nameof(Parallel)] = typeof(Parallel),
        [nameof(SendEvent)] = typeof(SendEvent),
        [nameof(SpeakItem)] = typeof(SpeakItem),
        [nameof(SpeakList)] = typeof(SpeakList),
        [nameof(Scroll)] = typeof(Scroll),
        [nameof(ScrollToComponent)] = typeof(ScrollToComponent),
        [nameof(ScrollToIndex)] = typeof(ScrollToIndex),
        [nameof(SetPage)] = typeof(SetPage),
        [nameof(AutoPage)] = typeof(AutoPage),
        [nameof(PlayMedia)] = typeof(PlayMedia),
        [nameof(ControlMedia)] = typeof(ControlMedia),
        [nameof(SetValue)] = typeof(SetValue),
        [nameof(AnimateItem)] = typeof(AnimateItem),
        [nameof(OpenURL)] = typeof(OpenURL),
        [nameof(SetFocus)] = typeof(SetFocus),
        [nameof(ClearFocus)] = typeof(ClearFocus),
        [nameof(Select)] = typeof(Select),
        [nameof(Finish)] = typeof(Finish),
        [nameof(Reinflate)] = typeof(Reinflate),
        [nameof(InsertItem)] = typeof(InsertItem),
        [nameof(RemoveItem)] = typeof(RemoveItem),
        [nameof(GoBackCommand)] = typeof(GoBackCommand),
        [nameof(ClearCommand)] = typeof(ClearCommand),
        [nameof(FollowPrimaryUserCommand)] = typeof(FollowPrimaryUserCommand),
        [nameof(GoToCenterCommand)] = typeof(GoToCenterCommand),
        [nameof(SetWakeWordResponseCommand)] = typeof(SetWakeWordResponseCommand),
        [nameof(StopMotionCommand)] = typeof(StopMotionCommand),
        [nameof(TurnToPrimaryUserCommand)] = typeof(TurnToPrimaryUserCommand),
        [nameof(PlayNamedChoreoCommand)] = typeof(PlayNamedChoreoCommand),
        [nameof(GetObjectCommand)] = typeof(GetObjectCommand),
        [nameof(WatchObjectCommand)] = typeof(WatchObjectCommand),
        [nameof(UnwatchObjectCommand)] = typeof(UnwatchObjectCommand),
        [nameof(UpdateArrayBindingRangeCommand)] = typeof(UpdateArrayBindingRangeCommand),
    };    
    protected override IDictionary<string, Type> DerivedTypes => _derivedTypes;
    protected override Type? DefaultType => typeof(CustomCommand);
}