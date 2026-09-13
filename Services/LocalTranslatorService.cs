using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace BlazorButtonDemo.Services;

public sealed class LocalTranslatorService(
    HttpClient httpClient,
    IOptions<LocalTranslatorOptions> options)
{
    private readonly LocalTranslatorOptions settings = options.Value;

    public async Task<string?> TryTranslateAsync(
        string text,
        string sourceLanguage,
        string targetLanguage,
        CancellationToken cancellationToken = default)
    {
        if (!settings.Enabled || !IsSupportedPair(sourceLanguage, targetLanguage))
        {
            return null;
        }

        var request = new LocalTranslationRequest(text, sourceLanguage, targetLanguage);
        using var response = await httpClient.PostAsJsonAsync(
            $"{settings.Endpoint.TrimEnd('/')}/translate",
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var result = await response.Content.ReadFromJsonAsync<LocalTranslationResponse>(cancellationToken);
        return result?.Translation;
    }

    private static bool IsSupportedPair(string sourceLanguage, string targetLanguage) =>
        (sourceLanguage, targetLanguage) is ("en", "es") or ("es", "en");

    private sealed record LocalTranslationRequest(
        [property: JsonPropertyName("text")] string Text,
        [property: JsonPropertyName("source_language")] string SourceLanguage,
        [property: JsonPropertyName("target_language")] string TargetLanguage);

    private sealed class LocalTranslationResponse
    {
        [JsonPropertyName("translation")]
        public string Translation { get; set; } = string.Empty;
    }
}