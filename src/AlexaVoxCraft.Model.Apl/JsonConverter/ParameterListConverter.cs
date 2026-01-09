namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class ParameterListConverter : SingleOrListConverter<Parameter>
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

public class ParameterValueCollectionConverter : APLValueCollectionConverter<Parameter>
{
    public ParameterValueCollectionConverter(bool alwaysOutputArray) : base(alwaysOutputArray) { }

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