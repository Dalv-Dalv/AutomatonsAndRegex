
using System.Text.Json.Serialization;

class TestString {
    [JsonPropertyName("input")] public string input;
    [JsonPropertyName("expected")] public bool expected;
}
