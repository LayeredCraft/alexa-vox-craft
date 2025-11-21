using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using NSubstitute.Core;

namespace AlexaVoxCraft.Smapi.Tests.TestKit.Extensions;

public static class HttpMessageHandlerExtensions
{
    private static readonly MethodInfo SendAsyncMethod =
        typeof(HttpMessageHandler)
            .GetMethod("SendAsync", BindingFlags.Instance | BindingFlags.NonPublic)!;

    extension(HttpMessageHandler handler)
    {
        /// <summary>
        /// Configures a response for HttpMessageHandler.SendAsync.
        /// If no predicate is supplied, it matches any HttpRequestMessage.
        /// </summary>
        public ConfiguredCall ReturnsResponse(
            HttpStatusCode statusCode,
            object? body = null,
            Func<HttpRequestMessage, bool>? predicate = null)
        {
            predicate ??= _ => true; // default: match any request

            var response = new HttpResponseMessage(statusCode)
            {
                Content = body is null ? null : JsonContent.Create(body)
            };

            // Register the call on the protected SendAsync method using NSubstitute Arg matchers
            SendAsyncMethod.Invoke(handler, [
                Arg.Is<HttpRequestMessage>(req => predicate(req)),
                Arg.Any<CancellationToken>()
            ]);

            // Apply the return value to that configured call
            return ((object)handler).Returns(Task.FromResult(response));
        }
    }
}