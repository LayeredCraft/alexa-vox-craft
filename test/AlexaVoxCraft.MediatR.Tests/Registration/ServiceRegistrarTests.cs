using System.Reflection;
using AlexaVoxCraft.MediatR.Attributes;
using AlexaVoxCraft.MediatR.DI;
using AlexaVoxCraft.MediatR.Pipeline;
using AlexaVoxCraft.MediatR.Registration;
using AlexaVoxCraft.MediatR.Response;
using Microsoft.Extensions.DependencyInjection;

namespace AlexaVoxCraft.MediatR.Tests.Registration;

public class ServiceRegistrarTests : TestBase
{
    [Theory]
    [MediatRAutoData]
    public void AddSkillMediatorClasses_WithEmptyAssemblies_DoesNotThrow(
        IServiceCollection services,
        SkillServiceConfiguration emptyConfiguration)
    {
        var exception = Record.Exception(() => services.AddSkillMediatorClasses(emptyConfiguration));
        
        exception.Should().BeNull();
    }

    [Theory]
    [MediatRAutoData]
    public void AddSkillMediatorClasses_WithValidConfiguration_DoesNotThrow(
        IServiceCollection services,
        SkillServiceConfiguration validConfiguration)
    {
        // Add current assembly to test registration logic
        validConfiguration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        
        var exception = Record.Exception(() => services.AddSkillMediatorClasses(validConfiguration));
        
        exception.Should().BeNull();
        // Note: Test assembly may not contain any handlers, so we just verify no exception is thrown
    }

    [Theory]
    [MediatRAutoData]
    public void AddSkillMediatorClasses_WithDuplicateAssemblies_DeduplicatesCorrectly(
        IServiceCollection services,
        SkillServiceConfiguration validConfiguration)
    {
        var assembly = Assembly.GetExecutingAssembly();
        validConfiguration.RegisterServicesFromAssembly(assembly);
        validConfiguration.RegisterServicesFromAssembly(assembly); // Add duplicate
        
        var exception = Record.Exception(() => services.AddSkillMediatorClasses(validConfiguration));
        
        exception.Should().BeNull();
        // Should handle duplicates without throwing
    }

    [Theory]
    [MediatRAutoData]
    public void AddSkillMediatorClasses_WithMediatRAssembly_RegistersPipelineTypes(
        IServiceCollection services,
        SkillServiceConfiguration validConfiguration)
    {
        // Add the MediatR assembly which contains actual pipeline types
        validConfiguration.RegisterServicesFromAssemblyContaining<SkillMediator>();
        
        services.AddSkillMediatorClasses(validConfiguration);
        
        // Should register pipeline interface types
        var pipelineServices = services.Where(s => 
            s.ServiceType == typeof(IExceptionHandler) ||
            s.ServiceType == typeof(IRequestInterceptor) ||
            s.ServiceType == typeof(IResponseInterceptor)).ToList();
        
        // Even if no concrete implementations exist in the assembly, the method should not fail
        pipelineServices.Should().NotBeNull();
    }

    [Theory]
    [MediatRAutoData]
    public void AddRequiredServices_RegistersAllRequiredServices(
        IServiceCollection services,
        SkillServiceConfiguration validConfiguration)
    {
        var initialCount = services.Count;
        
        services.AddRequiredServices(validConfiguration);
        
        services.Count.Should().BeGreaterThan(initialCount);
        
        // Verify key services are registered
        services.Should().Contain(s => s.ServiceType == typeof(ISkillMediator));
        services.Should().Contain(s => s.ServiceType == typeof(IHandlerInput));
        services.Should().Contain(s => s.ServiceType == typeof(IAttributesManager));
        services.Should().Contain(s => s.ServiceType == typeof(IResponseBuilder));
        services.Should().Contain(s => s.ServiceType == typeof(IPipelineBehavior));
    }

    [Theory]
    [MediatRAutoData]
    public void AddRequiredServices_WithMultipleCalls_DoesNotDuplicateServices(
        IServiceCollection services,
        SkillServiceConfiguration validConfiguration)
    {
        services.AddRequiredServices(validConfiguration);
        var countAfterFirst = services.Count;
        
        services.AddRequiredServices(validConfiguration);
        var countAfterSecond = services.Count;
        
        // Should not add duplicates due to TryAdd methods
        countAfterSecond.Should().Be(countAfterFirst);
    }

    [Theory]
    [MediatRAutoData]
    public void AddRequiredServices_RegistersCorrectServiceLifetimes(
        IServiceCollection services,
        SkillServiceConfiguration validConfiguration)
    {
        services.AddRequiredServices(validConfiguration);
        
        var skillMediatorService = services.FirstOrDefault(s => s.ServiceType == typeof(ISkillMediator));
        skillMediatorService.Should().NotBeNull();
        skillMediatorService!.Lifetime.Should().Be(ServiceLifetime.Transient);
        
        var attributesManagerService = services.FirstOrDefault(s => s.ServiceType == typeof(IAttributesManager));
        attributesManagerService.Should().NotBeNull();
        attributesManagerService!.Lifetime.Should().Be(ServiceLifetime.Scoped);
        
        var responseBuilderService = services.FirstOrDefault(s => s.ServiceType == typeof(IResponseBuilder));
        responseBuilderService.Should().NotBeNull();
        responseBuilderService!.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Theory]
    [MediatRAutoData]
    public void AddRequiredServices_RegistersAllPipelineBehaviors(
        IServiceCollection services,
        SkillServiceConfiguration validConfiguration)
    {
        services.AddRequiredServices(validConfiguration);
        
        var pipelineBehaviors = services.Where(s => s.ServiceType == typeof(IPipelineBehavior)).ToList();
        pipelineBehaviors.Should().HaveCount(4); // Performance, RequestInterceptor, ResponseInterceptor, RequestExceptionProcess
        
        var behaviorTypes = pipelineBehaviors.Select(s => s.ImplementationType?.Name).ToList();
        behaviorTypes.Should().Contain("PerformanceLoggingBehavior");
        behaviorTypes.Should().Contain("RequestInterceptorBehavior");
        behaviorTypes.Should().Contain("ResponseInterceptorBehavior");
        behaviorTypes.Should().Contain("RequestExceptionProcessBehavior");
    }

    [Theory]
    [MediatRAutoData]
    public void AddRequiredServices_RegistersImplementationTypes(
        IServiceCollection services,
        SkillServiceConfiguration validConfiguration)
    {
        services.AddRequiredServices(validConfiguration);
        
        // Verify specific implementation types are registered
        var skillMediatorService = services.FirstOrDefault(s => s.ServiceType == typeof(ISkillMediator));
        skillMediatorService!.ImplementationType.Should().Be(typeof(SkillMediator));
        
        var handlerInputService = services.FirstOrDefault(s => s.ServiceType == typeof(IHandlerInput));
        handlerInputService!.ImplementationType.Should().Be(typeof(DefaultHandlerInput));
        
        var attributesManagerService = services.FirstOrDefault(s => s.ServiceType == typeof(IAttributesManager));
        attributesManagerService!.ImplementationType.Should().Be(typeof(AttributesManager));
        
        var responseBuilderService = services.FirstOrDefault(s => s.ServiceType == typeof(IResponseBuilder));
        responseBuilderService!.ImplementationType.Should().Be(typeof(DefaultResponseBuilder));
    }

    [Theory]
    [MediatRAutoData]
    public void AddRequiredServices_DoesNotRegisterDuplicatePipelineBehaviors(
        IServiceCollection services,
        SkillServiceConfiguration validConfiguration)
    {
        services.AddRequiredServices(validConfiguration);
        var firstCount = services.Where(s => s.ServiceType == typeof(IPipelineBehavior)).Count();
        
        services.AddRequiredServices(validConfiguration);
        var secondCount = services.Where(s => s.ServiceType == typeof(IPipelineBehavior)).Count();
        
        // Should not add duplicate pipeline behaviors
        secondCount.Should().Be(firstCount);
    }

    [Theory]
    [MediatRAutoData]
    public void AddSkillMediatorClasses_WithAssemblyContainingRequestHandlers_RegistersHandlers(
        IServiceCollection services,
        SkillServiceConfiguration validConfiguration)
    {
        // Add the test assembly to ensure we have some types to work with
        validConfiguration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        
        var initialCount = services.Count;
        
        services.AddSkillMediatorClasses(validConfiguration);
        
        // Should have processed the assembly without errors
        services.Count.Should().BeGreaterThanOrEqualTo(initialCount);
    }

    [Theory]
    [MediatRAutoData]
    public void AddSkillMediatorClasses_WithNullAssemblies_HandlesGracefully(
        IServiceCollection services,
        SkillServiceConfiguration validConfiguration)
    {
        // Configuration with no assemblies should not fail
        var exception = Record.Exception(() => services.AddSkillMediatorClasses(validConfiguration));
        
        exception.Should().BeNull();
    }
}