using System.Text.Json.Serialization;

class RegexTest {
    [JsonPropertyName("name")] public string name;
    [JsonPropertyName("regex")] public string regex;
    [JsonPropertyName("test_strings")] public List<TestString> testStrings;
}