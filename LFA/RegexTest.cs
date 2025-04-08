using System.Text.Json.Serialization;

class RegexTest {
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("regex")] public string Regex { get; set; }
    [JsonPropertyName("test_strings")] public List<TestString> TestStrings { get; set; }
}