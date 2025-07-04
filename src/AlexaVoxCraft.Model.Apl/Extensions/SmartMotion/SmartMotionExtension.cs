﻿using System.Collections.Generic;

namespace AlexaVoxCraft.Model.Apl.Extensions.SmartMotion;

public class SmartMotionExtension : APLExtension
{
    public const string URL = "alexaext:smartmotion:10";
    public const string DeviceStateChangedEventName = "OnDeviceStateChanged";

    public SmartMotionExtension()
    {
        Uri = URL;
    }

    public SmartMotionExtension(string name) : this()
    {
        Name = name;
    }

    public void OnDeviceStateChanged(APLDocumentBase document, APLValue<IList<APLCommand>> commands)
    {
        document.AddHandler($"{Name}:{DeviceStateChangedEventName}", commands);
    }
}