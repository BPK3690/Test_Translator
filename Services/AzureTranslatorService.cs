using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace BlazorButtonDemo.Services;

public sealed class AzureTranslatorService(
    HttpClient httpClient,
    IOptions<AzureTranslatorOptions> options)
{
    private readonly AzureTranslatorOptions settings = options.Value;

    public async Task<string> TranslateAsync(
        string text,
        string sourceLanguage,
        string targetLanguage,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(settings.Endpoint) || string.IsNullOrWhiteSpace(settings.Key))
        {
            return await TranslateWithPublicProviderAsync(text, sourceLanguage, targetLanguage, cancellationToken);
        }

        var endpoint = settings.Endpoint.TrimEnd('/');
        var requestUri = $"{endpoint}/translate?api-version=3.0&from={Uri.EscapeDataString(sourceLanguage)}&to={Uri.EscapeDataString(targetLanguage)}";
        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = JsonContent.Create(new[] { new TranslationRequest(text) })
        };

        request.Headers.Add("Ocp-Apim-Subscription-Key", settings.Key);
        if (!string.IsNullOrWhiteSpace(settings.Region))
        {
            request.Headers.Add("Ocp-Apim-Subscription-Region", settings.Region);
        }

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"The translation service returned {(int)response.StatusCode} {response.ReasonPhrase}.");
        }

        var translations = await response.Content.ReadFromJsonAsync<TranslationResponse[]>(cancellationToken);
        var translatedText = translations?.FirstOrDefault()?.Translations?.FirstOrDefault()?.Text;

        return string.IsNullOrWhiteSpace(translatedText)
            ? throw new InvalidOperationException("The translation service returned no translated text.")
            : translatedText;
    }

    private async Task<string> TranslateWithPublicProviderAsync(
        string text,
        string sourceLanguage,
        string targetLanguage,
        CancellationToken cancellationToken)
    {
        var query = Uri.EscapeDataString(text);
        var requestUri = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={Uri.EscapeDataString(sourceLanguage)}&tl={Uri.EscapeDataString(targetLanguage)}&dt=t&q={query}";
        using var response = await httpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStreamAsync(cancellationToken));
        var translatedText = string.Join(
            string.Empty,
            document.RootElement[0].EnumerateArray().Select(segment => segment[0].GetString()));

        return string.IsNullOrWhiteSpace(translatedText)
            ? throw new InvalidOperationException("The public translation service returned no translated text.")
            : translatedText;
    }

    private sealed record TranslationRequest([property: JsonPropertyName("Text")] string Text);

    private sealed class TranslationResponse
    {
        [JsonPropertyName("translations")]
        public Translation[] Translations { get; set; } = [];
    }

    private sealed class Translation
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }

}