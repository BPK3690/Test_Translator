using System.Net;
using System.Net.Http.Headers;
using System.Text;
using BlazorButtonDemo.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace BlazorButtonDemo.Tests;

public sealed class AzureTranslatorServiceTests
{
    [Fact]
    public async Task TranslateAsync_UsesAzureWhenKeyIsConfigured()
    {
        var handler = new RecordingHandler("[{\"translations\":[{\"text\":\"Hola\",\"to\":\"es\"}]}]");
        var service = CreateService(handler, new AzureTranslatorOptions
        {
            Endpoint = "https://translator.test",
            Key = "temporary-key",
            Region = "westus"
        });

        var result = await service.TranslateAsync("Hello", "en", "es");

        Assert.Equal("Hola", result);
        Assert.Equal(HttpMethod.Post, handler.Request!.Method);
        Assert.Equal("https://translator.test/translate?api-version=3.0&from=en&to=es", handler.Request.RequestUri!.ToString());
        Assert.Equal("temporary-key", handler.Request.Headers.GetValues("Ocp-Apim-Subscription-Key").Single());
        Assert.Equal("westus", handler.Request.Headers.GetValues("Ocp-Apim-Subscription-Region").Single());
        Assert.Contains("Hello", handler.RequestBody);
    }

    [Fact]
    public async Task TranslateAsync_UsesPublicProviderWhenAzureKeyIsMissing()
    {
        var handler = new RecordingHandler("[[[\"Hola \",\"Hello \",null],[\"mundo\",\"world\",null]],null,\"en\"]");
        var service = CreateService(handler, new AzureTranslatorOptions
        {
            Endpoint = "https://api.cognitive.microsofttranslator.com"
        });

        var result = await service.TranslateAsync("Hello world", "en", "es");

        Assert.Equal("Hola mundo", result);
        Assert.Equal(HttpMethod.Get, handler.Request!.Method);
        Assert.Equal("https://translate.googleapis.com/translate_a/single?client=gtx&sl=en&tl=es&dt=t&q=Hello world", handler.Request.RequestUri!.ToString());
    }

    [Fact]
    public async Task TranslateAsync_ThrowsWhenProviderReturnsNoTranslation()
    {
        var handler = new RecordingHandler("[[]]");
        var service = CreateService(handler, new AzureTranslatorOptions());

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.TranslateAsync("Hello", "en", "es"));

        Assert.Equal("The public translation service returned no translated text.", exception.Message);
    }

    private static AzureTranslatorService CreateService(
        RecordingHandler handler,
        AzureTranslatorOptions options)
    {
        return new AzureTranslatorService(
            new HttpClient(handler),
            Options.Create(options));
    }

    private sealed class RecordingHandler(string responseBody) : HttpMessageHandler
    {
        public HttpRequestMessage? Request { get; private set; }
        public string RequestBody { get; private set; } = string.Empty;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Request = request;
            RequestBody = request.Content is null
                ? string.Empty
                : await request.Content.ReadAsStringAsync(cancellationToken);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseBody, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"))
            };
        }
    }
}
