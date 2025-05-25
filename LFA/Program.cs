// Ghita Vladut
// Grupa 151
// C# .NET 8.0

using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

internal class Program {
    static void Main() {
        CFGBuilder builder = new();
        Console.WriteLine("Enter your productions (S -> aSb | ..., use # for lambda and _ for spaces)");
        string line;
        while(!string.IsNullOrEmpty(line = Console.ReadLine())) {
            builder.AddProduction(line);
        }
        Console.WriteLine("Finished reading\n");

        Console.WriteLine("Randomly generated words:");
        CFG cfg = builder.Build();
        for(int i = 0; i < 10; i++) {
            Console.WriteLine($"\"{cfg.GenerateRandomWord(50, 100)}\"");
        }

        Console.WriteLine();

        while(true) {
            Console.WriteLine("Enter a word to check:");
            line = Console.ReadLine();
            if(cfg.ContainsWord(line)) {
                Console.WriteLine($"The CFG does contain \"{line}\"");
            } else {
                Console.WriteLine($"The CFG does NOT contain \"{line}\"");
            }

            string? res = cfg.DerivateWord(line);
            if(res == null) {
                Console.WriteLine("Could not derivate word");
            } else {
                Console.WriteLine("Derivation process:");
                Console.WriteLine(res);
            }

            Console.WriteLine("\n");
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

    static List<RegexTest> ReadTestsFromJSON(string path) {
        var options = new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true,
        };

        string json = File.ReadAllText(path);

        return JsonSerializer.Deserialize<List<RegexTest>>(json, options);
    }
}