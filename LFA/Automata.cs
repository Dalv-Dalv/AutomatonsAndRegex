// Ghita Vladut
// Grupa 151
// C# .NET 8.0

using System.Reflection.Metadata.Ecma335;
using System.Text;

public class Automata<State, Character> {
    public State[] states;
    public Character[] alphabet;
    public int initialStateIndex;
    public HashSet<State> finalStates;

    int[][][] transitionRule;

    public Automata(State[] states, Character[] alphabet, int[][][] transitionRule, State initialState, State[] finalStates) {
        this.states = states;
        this.alphabet = alphabet;
        initialStateIndex = Array.IndexOf(states, initialState);
        this.finalStates = new HashSet<State>();
        foreach(var state in finalStates) {
            this.finalStates.Add(state);
        }
        this.transitionRule = transitionRule;
    }
    public Automata(State[] states, Character[] alphabet, int[][] transitionRule, State initialState, State[] finalStates) {
        this.states = states;
        this.alphabet = alphabet;
        initialStateIndex = Array.IndexOf(states, initialState);
        this.finalStates = new HashSet<State>();
        foreach(var state in finalStates) {
            this.finalStates.Add(state);
        }
        this.transitionRule = new int[transitionRule.Length][][];
        for(int i = 0; i < transitionRule.Length; i++) {
            this.transitionRule[i] = new int[transitionRule[i].Length][];
            for(int j = 0; j < transitionRule[i].Length; j++) {
                this.transitionRule[i][j] = new int[] { transitionRule[i][j] };
            }
        }
    }

    public bool Validate(Character[] word) {
        // BFS
        Stack<(int, int)> crntStates = new Stack<(int, int)>();
        crntStates.Push((initialStateIndex, 0));

        while(crntStates.Count > 0) {
            var state = crntStates.Pop();

            int crntStateIndex = state.Item1;
            int i = state.Item2;

            if(i == word.Length) { 
                if(finalStates.Contains(states[crntStateIndex])) return true;
                continue;
            }

            if(!alphabet.Contains(word[i])) return false;
            int charIndex = Array.IndexOf(alphabet, word[i]);

            for(int j = 0; j < transitionRule[crntStateIndex][charIndex].Length; j++) {
                int nextStateIndex = transitionRule[crntStateIndex][charIndex][j];
                if(nextStateIndex < 0 || nextStateIndex > states.Length) continue;
                crntStates.Push((nextStateIndex, i + 1));
            }
        }
        return false;
    }

    public bool IsNondeterministic() {
        for(int i = 0; i < transitionRule.Length; i++) {
            for(int j = 0; j < transitionRule[i].Length; j++) {
                if(transitionRule[i][j].Length > 1) return true;
            }
        }
        return false;
    }

    public Automata<State, Character>? GetMinimizedVersion() {
        if(IsNondeterministic()) {
            Console.WriteLine("Sorry but I'm not smart enough to minimize NFA's yet");

            return null;
        }

        bool[][] pairs = new bool[states.Length][];

        // Pasul 1 si 2 din algoritmul Nyhil Nerode
        for(int i = 0; i < states.Length; i++) {
            pairs[i] = new bool[i];

            for(int j = 0; j < i; j++) {
                bool isValidPair = finalStates.Contains(states[i]) ^ finalStates.Contains(states[j]);

                pairs[i][j] = isValidPair;
            }
        }

        // Pasul 3 din algoritm
        for(int i = 0; i < pairs.Length; i++) {
            for(int j = 0; j < i; j++) {
                for(int c = 0; c < alphabet.Length; c++) {
                    (int, int) pair = (transitionRule[i][c][0], transitionRule[j][c][0]);
                    
                    if(pair.Item1 == -1 || pair.Item2 == -1) continue;
                    if(pair.Item1 == pair.Item2) continue;

                    if(pair.Item1 < pair.Item2) {
                        int aux = pair.Item1;
                        pair.Item1 = pair.Item2;
                        pair.Item2 = aux;
                    }


                    if(pairs[pair.Item1][pair.Item2]) {
                        pairs[i][j] = true;
                    }
                }
            }
        }

        for(int i = 0; i < pairs.Length; i++) {
            for(int j = 0; j < pairs[i].Length; j++) {
                Console.WriteLine($"({states[i]}, {states[j]}, {pairs[i][j]})");
            }
        }

        // Pasul 4 din algoritm:


        return null;
    }

    public override string ToString() {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(IsNondeterministic() ? "Nondeterministic Automaton" : "Deterministic Automaton");
        sb.AppendLine($"States: ({string.Join(',', states)})");
        sb.AppendLine($"Initial state: {states[initialStateIndex]}");
        sb.AppendLine($"Final states: {string.Join(',', finalStates)}");
        sb.AppendLine($"Alphabet: ({string.Join(',', alphabet)})");
        sb.AppendLine($"Transitions:");
        for(int i = 0; i < states.Length; i++) {
            sb.Append($"{states[i]} -> ");
            for(int j = 0; j < alphabet.Length; j++) {
                var connections = transitionRule[i][j].Where((k) => k >= 0).Select((k) => states[k]);

                sb.Append($"[{alphabet[j]}| {string.Join(',', connections)}] ".PadRight(9));
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
}



/*// DFS
        Queue<(int, int)> crntStates = new Queue<(int, int)>();
        crntStates.Enqueue((initialStateIndex, 0));

        while(crntStates.Count > 0) {
            var state = crntStates.Dequeue();

            int crntStateIndex = state.Item1;
            int i = state.Item2;

            if(i == word.Length) return finalStates.Contains(states[crntStateIndex]);

            if(!alphabet.Contains(word[i])) return false;
            int charIndex = Array.IndexOf(alphabet, word[i]);

            for(int j = 0; j < transitionRule[crntStateIndex][charIndex].Length; j++) {
                int nextStateIndex = transitionRule[crntStateIndex][charIndex][j];
                if(nextStateIndex < 0 || nextStateIndex > states.Length) continue;
                crntStates.Enqueue((nextStateIndex, i + 1));
            }
        }
        return false;*/