using AlexaVoxCraft.Model.Response;
using AutoFixture.Kernel;

namespace AlexaVoxCraft.TestKit.SpecimenBuilders;

public sealed class CardSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type)
        {
            if (type == typeof(SimpleCard))
                return CreateSimpleCard(context);
            
            if (type == typeof(StandardCard))
                return CreateStandardCard(context);
            
            if (type == typeof(AskForPermissionsConsentCard))
                return CreatePermissionsCard(context);
            
            if (type == typeof(LinkAccountCard))
                return CreateLinkAccountCard(context);
            
            if (type == typeof(CardImage))
                return CreateCardImage(context);
        }
        
        return new NoSpecimen();
    }
    
    private static SimpleCard CreateSimpleCard(ISpecimenContext context)
    {
        return new SimpleCard
        {
            Title = GenerateTitle(context),
            Content = GenerateContent(context)
        };
    }
    
    private static StandardCard CreateStandardCard(ISpecimenContext context)
    {
        return new StandardCard
        {
            Title = GenerateTitle(context),
            Content = GenerateContent(context),
            Image = CreateCardImage(context)
        };
    }
    
    private static AskForPermissionsConsentCard CreatePermissionsCard(ISpecimenContext context)
    {
        var card = new AskForPermissionsConsentCard();
        
        var availablePermissions = new[]
        {
            RequestedPermission.ReadHouseholdList,
            RequestedPermission.WriteHouseholdList,
            RequestedPermission.FullAddress,
            RequestedPermission.AddressCountryAndPostalCode
        };
        
        var permissionCount = context.Create<int>() % 3 + 1; // 1-3 permissions
        var selectedPermissions = availablePermissions
            .OrderBy(_ => context.Create<int>())
            .Take(permissionCount);
        
        foreach (var permission in selectedPermissions)
        {
            card.Permissions.Add(permission);
        }
        
        return card;
    }
    
    private static LinkAccountCard CreateLinkAccountCard(ISpecimenContext context)
    {
        return new LinkAccountCard();
    }
    
    private static CardImage CreateCardImage(ISpecimenContext context)
    {
        return new CardImage
        {
            SmallImageUrl = context.Create<Uri>().ToString(),
            LargeImageUrl = context.Create<Uri>().ToString()
        };
    }
    
    private static string GenerateTitle(ISpecimenContext context)
    {
        var words = new[] { "Welcome", "Hello", "Greetings", "Today", "Weather", "News", "Update" };
        var wordCount = context.Create<int>() % 3 + 1; // 1-3 words
        var selectedWords = words.OrderBy(_ => context.Create<int>()).Take(wordCount);
        var title = string.Join(" ", selectedWords);
        
        // Ensure title doesn't exceed Alexa's 50 character limit
        return title.Length > 50 ? title.Substring(0, 47) + "..." : title;
    }
    
    private static string GenerateContent(ISpecimenContext context)
    {
        var sentences = new[]
        {
            "This is your daily update.",
            "Here's what's happening today.",
            "Welcome to your skill.",
            "Thanks for using our service.",
            "Have a great day!"
        };
        
        var sentenceCount = context.Create<int>() % 3 + 1; // 1-3 sentences
        var selectedSentences = sentences.OrderBy(_ => context.Create<int>()).Take(sentenceCount);
        var content = string.Join(" ", selectedSentences);
        
        // Ensure content doesn't exceed reasonable limits
        return content.Length > 200 ? content.Substring(0, 197) + "..." : content;
    }
}