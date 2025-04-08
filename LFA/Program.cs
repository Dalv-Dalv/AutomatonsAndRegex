// Ghita Vladut
// Grupa 151
// C# .NET 8.0

using System.Text.Json;

internal class Program {
    static void Main(string[] args) {
        var tests = ReadTestsFromJSON(@"C:\Dalv\School\University\Classes\Semestrul2\LFA\Sarcini\LFA-Assignment2_Regex_DFA.json");

        Thread.Sleep(2000);

        foreach(var test in tests) {
            Console.WriteLine($"Parsing regex: {test.Regex}");
            AutomataBuilder.BuildFromRegex(test.Regex);
        }
    }

    
    //static RegexToken? ProcessRegexToken(string input, ref int i) {
    //    if(i >= input.Length) return null;

    //    RegexToken token = new RegexToken();

    //    int startIndex = i;
    //    if(input[i] == '(') {
    //        token.op1 = ProcessRegexToken(input, ref i);
    //    } else {
    //        token.op1 = new RegexToken{ expression = input.Substring(i, 1) };
    //        i++;
    //    }

    //    var operation = GetRegexOperator(input[i]);

    //    if(OperatorsString.Contains(input[i])) {

    //        if(IsUnaryOperator(input[i])) {
    //            token.operation = (RegexOperators)input[i];
    //        }
    //    }


    //    i++;
    //    return token;
    //}


    //static void ProcessRegexTest(RegexTest test) {
    //    string regex = test.Regex;

    //    int i = 0;
    //    var token = ProcessRegexToken(regex, ref i);
    //}

    static List<RegexTest> ReadTestsFromJSON(string path) {
        var options = new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true,
        };

        string json = File.ReadAllText(path);

        return JsonSerializer.Deserialize<List<RegexTest>>(json, options);
    }
}