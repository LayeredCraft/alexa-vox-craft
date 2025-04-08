﻿using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AlexaVoxCraft.Model.Serialization;

namespace Alexa.NET.APL.Tests;

public class ActionHandler : HttpMessageHandler
{
    public Func<HttpRequestMessage, Task<HttpResponseMessage>> Run { get; }

    private static readonly JsonSerializerOptions _jsonOptions = AlexaJsonOptions.DefaultOptions;

    public ActionHandler(Action<HttpRequestMessage> run, object response, HttpStatusCode code = HttpStatusCode.OK)
    {
        Run = req =>
        {
            run(req);
            return Task.FromResult(new HttpResponseMessage(code)
            {
                Content = SerializeToJsonContent(response)
            });
        };
    }

    public ActionHandler(Action<HttpRequestMessage> run, HttpStatusCode code = HttpStatusCode.OK)
    {
        Run = req =>
        {
            run(req);
            return Task.FromResult(new HttpResponseMessage(code));
        };
    }

    public ActionHandler(Func<HttpRequestMessage, Task> run, object response, HttpStatusCode code = HttpStatusCode.OK)
    {
        Run = async req =>
        {
            await run(req);
            return new HttpResponseMessage(code)
            {
                Content = SerializeToJsonContent(response)
            };
        };
    }

    public ActionHandler(Func<HttpRequestMessage, Task> run, HttpStatusCode code = HttpStatusCode.OK)
    {
        Run = async req =>
        {
            await run(req);
            return new HttpResponseMessage(code);
        };
    }

    public ActionHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> run)
    {
        Run = run;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return await Run(request);
    }

    private static StringContent SerializeToJsonContent(object response)
    {
        var json = response is string s
            ? s
            : JsonSerializer.Serialize(response, _jsonOptions);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
