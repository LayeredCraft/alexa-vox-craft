namespace AlexaVoxCraft.Model.Tests;

public static class TestHelper
{
    public static async Task VerifyRequestObject<TRequest>(TRequest request)
    {
        await Verify(request).DisableDiff();
    }
}