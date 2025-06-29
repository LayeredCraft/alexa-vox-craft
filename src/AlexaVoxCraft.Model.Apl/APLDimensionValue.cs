using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;

namespace AlexaVoxCraft.Model.Apl;

[JsonConverter(typeof(APLDimensionValueConverter))]
public class APLDimensionValue : APLDimensionValue<Dimension>
{
    public APLDimensionValue()
    {
    }

    public APLDimensionValue(Dimension dimension) : base(dimension)
    {
    }

    public APLDimensionValue(string value)
    {
        var dimension = Dimension.From(value);
        if (dimension == null)
        {
            Expression = value;
        }

        Value = dimension;
    }

    public static implicit operator APLDimensionValue(int value)
    {
        return new APLDimensionValue(new AbsoluteDimension(value, "dp"));
    }

    public static implicit operator APLDimensionValue(string value)
    {
        return new APLDimensionValue(value);
    }

    public static implicit operator APLDimensionValue(Dimension value)
    {
        return new APLDimensionValue(value);
    }
}