using AlexaVoxCraft.Model.Apl;
using AlexaVoxCraft.Model.Request;

namespace AlexaVoxCraft.Model.WebApiGames.Tests;

public sealed class HTMLInterfaceTests
{
    [Fact]
    public void Supported_WhenKeyPresent()
    {
        RequestWithInterfaces("Alexa.Presentation.HTML").HTMLSupported().Should().BeTrue();
    }

    [Fact]
    public void Unsupported_WhenKeyAbsent()
    {
        RequestWithInterfaces("Alexa.Presentation.APL").HTMLSupported().Should().BeFalse();
    }

    [Fact]
    public void Unsupported_WhenSupportedInterfacesNull()
    {
        var request = new SkillRequest
        {
            Context = new Context
            {
                System = new AlexaSystem
                {
                    Device = new Device { DeviceID = "d", SupportedInterfaces = null! }
                }
            }
        };

        request.HTMLSupported().Should().BeFalse();
    }

    [Fact]
    public void Unsupported_WhenDeviceNull()
    {
        var request = new SkillRequest
        {
            Context = new Context { System = new AlexaSystem { Device = null! } }
        };

        request.HTMLSupported().Should().BeFalse();
    }

    [Fact]
    public void Unsupported_WhenSystemNull()
    {
        var request = new SkillRequest { Context = new Context { System = null! } };

        request.HTMLSupported().Should().BeFalse();
    }

    [Fact]
    public void Unsupported_WhenContextNull()
    {
        var request = new SkillRequest { Context = null! };

        request.HTMLSupported().Should().BeFalse();
    }

    [Fact]
    public void Unsupported_WhenRequestNull()
    {
        SkillRequest request = null!;

        request.HTMLSupported().Should().BeFalse();
    }

    [Fact]
    public void CoexistsWithAPLSupported()
    {
        var request = RequestWithInterfaces("Alexa.Presentation.HTML", "Alexa.Presentation.APL");

        request.HTMLSupported().Should().BeTrue();
        request.APLSupported().Should().BeTrue();
    }

    [Fact]
    public void HTMLOnlyIsNotAPLSupported()
    {
        var request = RequestWithInterfaces("Alexa.Presentation.HTML");

        request.HTMLSupported().Should().BeTrue();
        request.APLSupported().Should().BeFalse();
    }

    private static SkillRequest RequestWithInterfaces(params string[] names)
    {
        var interfaces = names.ToDictionary(name => name, _ => (object)new { });
        return new SkillRequest
        {
            Context = new Context
            {
                System = new AlexaSystem
                {
                    Device = new Device { DeviceID = "d", SupportedInterfaces = interfaces }
                }
            }
        };
    }
}
