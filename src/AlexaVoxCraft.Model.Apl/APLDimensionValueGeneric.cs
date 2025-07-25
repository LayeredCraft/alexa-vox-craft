﻿using System.Linq;

namespace AlexaVoxCraft.Model.Apl;

public class APLDimensionValue<T> : APLValue<T> where T : Dimension
{
    public APLDimensionValue() { }

    public APLDimensionValue(T dimension) : base(dimension) { }

    public override object GetValue()
    {
        if (Value == null)
        {
            return null;
        }

        var value = Value.GetValue().ToString();
        if (value.All(char.IsDigit))
        {
            return int.Parse(value);
        }

        return value;
    }
}