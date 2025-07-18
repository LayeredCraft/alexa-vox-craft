﻿namespace AlexaVoxCraft.Model.Apl;

public class AbsoluteDimension : Dimension
{
    public AbsoluteDimension(int number, string unit)
    {
        Number = number;
        Unit = unit;
    }

    public static explicit operator AbsoluteDimension(int value)
    {
        return new AbsoluteDimension(value,string.Empty);
    }

    public string Unit { get; set; }

    public int Number { get; set; }

    public override object GetValue()
    {
        return $"{Number}{Unit}";
    }
}