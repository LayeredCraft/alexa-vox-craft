using System.Collections.Generic;

namespace AlexaVoxCraft.Model.Apl.Extensions.EntitySensing;

public class EntitySensingExtension : APLExtension
{
    public const string URL = "alexaext:entitysensing:10";
    public const string EntitySensingStateChangedEventName = "OnEntitySensingStateChanged";
    public const string PrimaryUserChangedEventName = "OnPrimaryUserChanged";

    public EntitySensingExtension()
    {
        Uri = URL;
    }

    public EntitySensingExtension(string name) : this()
    {
        Name = name;
    }

    public void OnEntitySensingStateChanged(APLDocumentBase document, APLValue<IList<APLCommand>> commands)
    {
        document.AddHandler($"{Name}:{EntitySensingStateChangedEventName}", commands);
    }

    public void OnPrimaryUserChanged(APLDocumentBase document, APLValue<IList<APLCommand>> commands)
    {
        document.AddHandler($"{Name}:{PrimaryUserChangedEventName}", commands);
    }
}