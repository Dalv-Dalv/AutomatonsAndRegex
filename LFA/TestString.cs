
using System.Text.Json.Serialization;

class TestString {
    [JsonPropertyName("input")] public string Input { get; set; }
    [JsonPropertyName("expected")] public bool Expected { get; set; }
}
