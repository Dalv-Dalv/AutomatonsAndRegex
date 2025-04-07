// Ghita Vladut
// Grupa 151
// C# .NET 8.0

using System.Text.Json;

internal class Program {
    static void Main(string[] args) {
        var tes = ReadTestsFromJSON(@"C:\Dalv\School\University\Classes\Semestrul2\LFA\Sarcini\LFA-Assignment2_Regex_DFA.json");

        Console.WriteLine(tes);

        //Automata<string, char> automata;
        //try {
        //    automata = AutomataBuilder.ReadFromConfigFile(@"C:\Dalv\School\University\Classes\Semestrul2\LFA\Sarcini\dfaMinTest.config");
        //    Console.WriteLine("Automata is valid");
        //} catch(Exception ex) {
        //    Console.WriteLine("Automata is not valid:");
        //    Console.WriteLine($"{ex.Message}");
        //    return;
        //}

        //Console.WriteLine($"Automata is a {(automata.IsNondeterministic() ? "NFA" : "DFA")}" );

        //automata.GetMinimizedVersion();
    }

    static void RunTest(Automata<string, char> automata, string test) {
        Console.WriteLine($"{automata.Validate(test.ToCharArray())} {test}");
    }

    static List<RegexTest> ReadTestsFromJSON(string path) {
        var options = new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true,
        };

        string json = File.ReadAllText(path);

        Console.WriteLine(json);

        return JsonSerializer.Deserialize<List<RegexTest>>(json, options);
    }
}
