// Ghita Vladut
// Grupa 151
// C# .NET 8.0


using System.Text;
public static class AutomataBuilder {
    public static Automata<string, char> ReadFromConfigFile(string path) {
        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);

        string[] states = null;
        char[] alphabet = null;
        string initialState = null;
        string[] finalStates = null;
        List<(string, char, string)> transitions = null;

        using(var sr = new StreamReader(fs, Encoding.UTF8)) {
            string line;
            while((line = sr.ReadLine()) != null) {
                if(line.StartsWith('#')) continue;

                if(line.Contains("Sigma")) {
                    if(alphabet != null) throw new Exception("Alphabet has already been initialized");
                    alphabet = ReadAlphabet(sr);
                    continue;
                }

                if(line.Contains("States")) {
                    if(states != null) throw new Exception("States have already been initialized");
                    (states, initialState, finalStates) = ReadStates(sr);
                    continue;
                }

                if(line.Contains("Transitions")) {
                    if(transitions != null) throw new Exception("Transitions have already been initialized");
                    transitions = ReadTransitions(sr);

                    continue;
                }
            }
        }

        if(states == null || alphabet == null || initialState == null || finalStates == null || transitions == null) throw new Exception("Not all parts of the automata have been defined");

        
        int[][][] transitionRules = ExtractTransitionRule(transitions, states, alphabet);

        return new Automata<string, char>(states, alphabet, transitionRules, initialState, finalStates);
    }

    //      states   initial  final states
    static (string[], string, string[]) ReadStates(StreamReader sr) {
        List<string> states = new List<string>();
        string initialState = null;
        List<string> finalStates = new List<string>();

        string line;
        while((line = sr.ReadLine()) != null) {
            if(line.StartsWith('#') || string.IsNullOrEmpty(line)) continue;
            if(line.Contains("End")) break;

            var split = line.Split(',');

            var state = split[0].Trim();

            var info = String.Join("",split.Skip(1).ToArray());

            if(split.Length > 1) {
                if(info.Contains('S')) {
                    if(initialState != null) throw new Exception("Initial state has already been defined");
                    initialState = state;
                }

                if(info.Contains('F')) finalStates.Add(state);
            }

            if(states.Contains(state)) throw new Exception($"State \'{state}\' is duplicated");

            states.Add(state);
        }
        return (states.ToArray(), initialState, finalStates.ToArray());
    }
    static char[] ReadAlphabet(StreamReader sr) {
        string line;
        List<char> characters = new List<char>();
        while((line = sr.ReadLine()) != null) {
            if(line.StartsWith('#') || string.IsNullOrEmpty(line)) continue;
            if(line.Contains("End")) break;

            characters.Add(line.Trim()[0]);
        }
        return characters.ToArray();
    }
    static List<(string, char, string)> ReadTransitions(StreamReader sr) {
        var transitions = new List<(string, char, string)>();

        string line;
        while((line = sr.ReadLine()) != null) {
            if(line.StartsWith('#') || string.IsNullOrEmpty(line)) continue;
            if(line.Contains("End")) break;

            var split = line.Split(',');

            var tuple = (split[0].Trim(), split[1].Trim()[0], split[2].Trim());

            if(transitions.Contains(tuple)) continue;

            transitions.Add(tuple);
        }

        return transitions;
    }


    static int[][][] ExtractTransitionRule(List<(string, char, string)> transitions, string[] states, char[] alphabet) {
        int[][][] res = new int[states.Length][][];

        List<int>[][] transitionRules = new List<int>[states.Length][];
        for(int i = 0; i < states.Length; i++) {
            transitionRules[i] = new List<int>[alphabet.Length];
            res[i] = new int[alphabet.Length][];

            for(int j = 0; j < alphabet.Length; j++) {
                transitionRules[i][j] = new List<int>();
            }
        }

        foreach(var transition in transitions) {
            var start = Array.IndexOf(states, transition.Item1);
            var character = Array.IndexOf(alphabet, transition.Item2);
            var end = Array.IndexOf(states, transition.Item3);

            if(start < 0 || end < 0) throw new Exception($"State \'{transition.Item1}\' is undefined");
            if(character < 0) throw new Exception($"Character \'{transition.Item2}\' is undefined");

            transitionRules[start][character].Add(end);
        }

        for(int i = 0; i < states.Length; i++) {
            for(int j = 0; j < alphabet.Length; j++) {
                res[i][j] = transitionRules[i][j].ToArray();
            }
        }

        return res;
    }
}
