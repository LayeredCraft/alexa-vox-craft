namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class ParameterListConverter : APLValueCollectionConverter<Parameter>
{
    public ParameterListConverter(bool alwaysOutputArray) : base(alwaysOutputArray) { }

    protected override object OutputArrayItem(Parameter param)
    {
        if (param.Default == null && string.IsNullOrWhiteSpace(param.Description) &&
            param.ShouldSerializeType() == false)
        {
            return param.Name;
        }

        return param;
    }
}