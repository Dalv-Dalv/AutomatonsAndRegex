// Ghita Vladut
// Grupa 151
// C# .NET 8.0

using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

internal class Program {
    static void Main(string[] args) {
        var tests = ReadTestsFromJSON(@"C:\Dalv\School\University\Classes\Semestrul2\LFA\Sarcini\LFA-Assignment2_Regex_DFA.json");

        //Console.WriteLine($"Parsing regex: {tests[2].Regex}");
        //var automat = AutomataBuilder.BuildFromRegex(tests[2].Regex);
        //foreach(var test in tests[2].TestStrings) {
        //    var res = automat.Validate(test.Input.ToCharArray());
        //    if(res != test.Expected) {
        //        Console.ForegroundColor = ConsoleColor.Red;
        //    } else {
        //        Console.ForegroundColor = ConsoleColor.Green;
        //    }

        //    Console.WriteLine($"\"{test.Input}\"");

        //    Console.ForegroundColor = ConsoleColor.White;
        //}

        foreach(var test in tests) {
            Console.WriteLine($"Parsing regex: {test.Regex}");
            var automat = AutomataBuilder.BuildFromRegex(test.Regex);

            Console.WriteLine(automat);

            RunTest(automat, test);
        }
    }

    static void RunTest(Automata<string, char> automat, RegexTest test) {
        foreach(var inp in test.TestStrings) {
            var res = automat.Validate(inp.Input.ToCharArray());
            if(res != inp.Expected) {
                Console.ForegroundColor = ConsoleColor.Red;
            } else {
                Console.ForegroundColor = ConsoleColor.Green;
            }

            Console.WriteLine($"\"{inp.Input}\" Expected ({inp.Expected}) Got ({res})");

            Console.ForegroundColor = ConsoleColor.White;
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