namespace BlazorButtonDemo.Services;

public sealed class AzureTranslatorOptions
{
    public const string SectionName = "AzureTranslator";

    public string Endpoint { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
}