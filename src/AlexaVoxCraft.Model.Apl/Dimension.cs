﻿using System.Linq;

namespace AlexaVoxCraft.Model.Apl;

public class Dimension
{
    public virtual object GetValue()
    {
        return null;
    }

    public static explicit operator Dimension(string value)
    {
        return From(value);
    }

    public static Dimension From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (int.TryParse(value, out var intvalue))
        {
            return (AbsoluteDimension) intvalue;
        }

        if (value.Last() == '%')
        {
            return new RelativeDimension(int.Parse(value.Substring(0, value.Length - 1)));
        }

        if (char.IsNumber(value[0]))
        {
            var split = value.Length;
            for (var splitTest = 0; splitTest < value.Length; splitTest++)
            {
                if (!char.IsDigit(value[splitTest]))
                {
                    split = splitTest;
                    break;
                }
            }

            return new AbsoluteDimension(int.Parse(value.Substring(0, split)), value.Substring(split));
        }

        if (value == "auto")
        {
            return new SpecialDimension(value);
        }

        return null;
    }
}