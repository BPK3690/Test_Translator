namespace BlazorButtonDemo.Services;

public sealed class LocalTranslatorOptions
{
    public const string SectionName = "LocalTranslator";

    public bool Enabled { get; set; }
    public string Endpoint { get; set; } = "http://127.0.0.1:8000";
}