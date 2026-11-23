using System.Diagnostics;

namespace AlexaVoxCraft.Model.WebApiGames.Tests;

/// <summary>
/// Proves WebApiGamesSupport.Add() idempotence and order independence against APLSupport.Add()/
/// InSkillPurchasingSupport.Add() in fresh child processes, because the underlying registries are
/// global and have no public reset.
/// </summary>
public sealed class RegistrationProbeTests
{
    [Theory]
    [InlineData("webapigames")]
    [InlineData("webapigames,webapigames")]
    [InlineData("webapigames,apl,isp")]
    [InlineData("apl,isp,webapigames")]
    [InlineData("apl,webapigames,isp")]
    [InlineData("isp,webapigames,apl")]
    public async Task ProbeSucceedsForOrdering(string order)
    {
        var probeAssembly = LocateProbeAssembly();
        File.Exists(probeAssembly).Should().BeTrue($"the registration probe should have been built at {probeAssembly}");

        var startInfo = new ProcessStartInfo("dotnet")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        startInfo.ArgumentList.Add(probeAssembly);
        startInfo.ArgumentList.Add(order);

        using var process = Process.Start(startInfo)!;
        var cancellationToken = TestContext.Current.CancellationToken;
        var standardOutput = await process.StandardOutput.ReadToEndAsync(cancellationToken);
        var standardError = await process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);

        process.ExitCode.Should().Be(0, $"probe output:\n{standardOutput}\n{standardError}");
    }

    private static string LocateProbeAssembly()
    {
        var baseDirectory = new DirectoryInfo(AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar));
        var targetFramework = baseDirectory.Name;
        var configuration = baseDirectory.Parent!.Name;
        var testProjectDirectory = baseDirectory.Parent!.Parent!.Parent!.FullName;

        return Path.Combine(
            testProjectDirectory, "RegistrationProbe", "bin", configuration, targetFramework, "RegistrationProbe.dll");
    }
}
