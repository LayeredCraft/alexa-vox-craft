using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Response.Directive.Templates;

namespace AlexaVoxCraft.Model.Serialization;

public class AlexaTypeResolver : DefaultJsonTypeInfoResolver
{
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var typeInfo = base.GetTypeInfo(type, options);

        if (typeInfo.Type == typeof(ResponseBody))
        {
            var prop = typeInfo.Properties.FirstOrDefault(p => p.Name == "directives");
            if (prop != null)
            {
                prop.ShouldSerialize = (obj, _) =>
                {
                    var response = (ResponseBody)obj;
                    return response.Directives is { Count: > 0 };
                };
            }
        }
        else if (typeInfo.Type == typeof(Reprompt))
        {
            var prop = typeInfo.Properties.FirstOrDefault(p => p.Name == "directives");
            if (prop != null)
            {
                prop.ShouldSerialize = (obj, _) =>
                {
                    var response = (Reprompt)obj;
                    return response.Directives is { Count: > 0 };
                };
            }
        }
        else if (typeInfo.Type == typeof(ImageSource))
        {
            var widthProp = typeInfo.Properties.FirstOrDefault(p => p.Name == "widthPixels");
            if (widthProp is not null)
            {
                widthProp.ShouldSerialize = (obj, _) =>
                {
                    var imageSource = (ImageSource)obj;
                    return imageSource.Width > 0;
                };
            }

            var heightProp = typeInfo.Properties.FirstOrDefault(p => p.Name == "heightPixels");
            if (heightProp is not null)
            {
                heightProp.ShouldSerialize = (obj, _) =>
                {
                    var imageSource = (ImageSource)obj;
                    return imageSource.Height > 0;
                };
            }
        }

        return typeInfo;
    }
}