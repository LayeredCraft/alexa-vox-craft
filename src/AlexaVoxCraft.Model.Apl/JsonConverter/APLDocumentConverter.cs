using System;
using System.Collections.Generic;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class APLDocumentConverter : BasePolymorphicConverter<APLDocumentReference>
{
    // Leaving this commented out for now. I'm not sure why they were doing a ToUpper() on the type string,
    // but I want to keep this in case we need to do something similar in the future.
    
    // public override APLDocumentReference ReadJson(JsonReader reader, Type objectType,
    //     APLDocumentReference existingValue, bool hasExistingValue,
    //     JsonSerializer serializer)
    // {
    //     var jObject = JObject.Load(reader);
    //     var documentType = jObject.Value<string>("type");
    //
    //     switch (documentType.ToUpper())
    //     {
    //         case "APL":
    //             var apl = new APLDocument();
    //             serializer.Populate(jObject.CreateReader(), apl);
    //             return apl;
    //         case "APLT":
    //             var aplt = new APLTDocument();
    //             serializer.Populate(jObject.CreateReader(), aplt);
    //             return aplt;
    //         case "APLA":
    //             var apla = new APLADocument();
    //             serializer.Populate(jObject.CreateReader(), apla);
    //             return apla;
    //         case "LINK":
    //             var link = new APLDocumentLink();
    //             serializer.Populate(jObject.CreateReader(), link);
    //             return link;
    //     }
    //
    //     throw new InvalidOperationException($"Unknown APL Document type {documentType}");
    // }

    protected override IDictionary<string, Type> DerivedTypes => new Dictionary<string, Type>
    {
        { APLDocument.DocumentType, typeof(APLDocument) },
        { APLTDocument.DocumentType, typeof(APLTDocument) },
        { APLADocument.DocumentType, typeof(APLADocument) },
        { APLDocumentLink.DocumentType, typeof(APLDocumentLink) }
    };
}