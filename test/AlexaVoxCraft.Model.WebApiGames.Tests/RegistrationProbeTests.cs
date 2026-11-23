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
        const string probeFileName = "RegistrationProbe.dll";
        var baseDirectory = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        // Shared-output layout (CI): every project is written to a common <publish>/<tfm>/ directory, so
        // the probe sits next to the test assembly.
        var adjacentProbe = Path.Join(baseDirectory, probeFileName);
        if (File.Exists(adjacentProbe))
        {
            return adjacentProbe;
        }

        // Default local layout: the test assembly is under <testProject>/bin/<configuration>/<tfm>/ and the
        // probe under <testProject>/RegistrationProbe/bin/<configuration>/<tfm>/. Path.Join never drops an
        // earlier argument when a later one is rooted.
        var baseDirectoryInfo = new DirectoryInfo(baseDirectory);
        var targetFramework = baseDirectoryInfo.Name;
        var configuration = baseDirectoryInfo.Parent!.Name;
        var testProjectDirectory = baseDirectoryInfo.Parent!.Parent!.Parent!.FullName;

        return Path.Join(testProjectDirectory, "RegistrationProbe", "bin", configuration, targetFramework, probeFileName);
    }
}
