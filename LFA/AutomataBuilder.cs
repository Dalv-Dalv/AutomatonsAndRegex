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


    // FOR REGEX BUILDING:
    class Node {
        public int id;
        public List<(Node, char?)> connections;
        public Node(int id) {
            this.id = id;
            connections = new List<(Node, char?)>();
        }

        public void AddConnection(Node node, char? connection) {
            connections.Add((node, connection));
        }

        public override string ToString() {
            return $"({id})";
        }
    }

    class Token {
        public Node startNode;
        public Node endNode;
        Token(Node startNode, Node endNode) {
            this.startNode = startNode;
            this.endNode = endNode;
        }

        public Token(char symbol, ref int i) {
            startNode = new Node(i++);
            endNode = new Node(i++);
            startNode.connections.Add((endNode, symbol));
        }

        public Token(Token lhs, Token? rhs, char operation, ref int i) {
            switch(operation) {
                case '.':
                    startNode = lhs.startNode;
                    lhs.endNode.AddConnection(rhs.startNode, null);
                    endNode = rhs.endNode;
                    break;
                case '|':
                    startNode = new Node(i++);
                    endNode = new Node(i++);

                    startNode.AddConnection(lhs.startNode, null);
                    startNode.AddConnection(rhs.startNode, null);
                    lhs.endNode.AddConnection(endNode, null);
                    rhs.endNode.AddConnection(endNode, null);
                    break;

                case '*':
                    startNode = new Node(i++);
                    endNode = new Node(i++);

                    startNode.AddConnection(lhs.startNode, null);
                    lhs.endNode.AddConnection(lhs.startNode, null);
                    lhs.endNode.AddConnection(endNode, null);
                    startNode.AddConnection(endNode, null);
                    break;
                case '?':
                    startNode = new Node(i++);
                    endNode = new Node(i++);

                    startNode.AddConnection(lhs.startNode, null);
                    lhs.endNode.AddConnection(endNode, null);
                    startNode.AddConnection(endNode, null);
                    break;
                case '+':
                    startNode = new Node(i++);
                    endNode = new Node(i++);

                    Token copy = DeepCopy(lhs, ref i);
                    startNode.AddConnection(copy.startNode, null);
                    copy.endNode.AddConnection(lhs.startNode, null);
                    lhs.endNode.AddConnection(lhs.startNode, null);
                    lhs.endNode.AddConnection(endNode, null);
                    break;
                default:
                    throw new Exception("Operation not recognzied");
            }
        }

        public override string ToString() {
            return $"{startNode}...{endNode}";
        }

        public static Token DeepCopy(Token crnt, ref int i) {
            Dictionary<Node, Node> visited = new();
            DeepCopy(crnt.startNode, visited, ref i);

            return new Token(visited[crnt.startNode], visited[crnt.endNode]);
        }

        static Node DeepCopy(Node node, Dictionary<Node, Node> visited, ref int i) {
            if(node == null) return null;

            if(visited.ContainsKey(node)) return visited[node];

            Node copy = new(i++);
            visited[node] = copy;
            foreach(var item in node.connections) {
                copy.AddConnection(DeepCopy(item.Item1, visited, ref i), item.Item2);
            }

            return copy;
        }
    }

    static Token GetLambdaNFAToken(string postfixRegex, ref int nodesCount) {
        Stack<Token> tokens = new();

        bool IsOperator(char c) {
            return ".?*+|".Contains(c);
        }
        bool IsBinaryOperator(char c) {
            return ".|".Contains(c);
        }

        for(int i = 0; i < postfixRegex.Length; i++) {
            if(IsOperator(postfixRegex[i])) {
                Token lhs;
                Token? rhs = null;

                lhs = tokens.Pop();

                if(IsBinaryOperator(postfixRegex[i])) {
                    rhs = lhs;
                    lhs = tokens.Pop();
                }

                tokens.Push(new Token(lhs, rhs, postfixRegex[i], ref nodesCount));

            } else {
                tokens.Push(new Token(postfixRegex[i], ref nodesCount));
            }
        }

        if(tokens.Count != 1) throw new Exception("Postfix expression is wrong");

        return tokens.Pop();
    }

    static void PrintTokenHierarchy(Token token) {
        HashSet<Node> visited = new();
        Stack<(Node, int)> toExplore = new();

        toExplore.Push((token.startNode, 0));

        while(toExplore.Count > 0) {
            (var node, var indent)= toExplore.Pop();
            visited.Add(node);

            foreach(var connection in node.connections) {
                var otherNode = connection.Item1;

                Console.WriteLine($"{new string(' ', indent * 4)}{node}--{(connection.Item2 == null ? 'L' : connection.Item2)}-->{otherNode}");

                if(visited.Contains(otherNode)) continue;
                toExplore.Push((otherNode, indent + 1));
            }
        }
    }

    static void DFSDo(Node origin, Action<Node> action, Func<(Node node, char? connection), bool> validate) {
        HashSet<Node> visited = new();
        Stack<Node> toExplore = new();

        toExplore.Push(origin);

        while(toExplore.Count > 0) {
            var node = toExplore.Pop();
            action(node);

            visited.Add(node);

            foreach(var connection in node.connections) {
                if(visited.Contains(connection.Item1) || !validate(connection)) continue;

                toExplore.Push(connection.Item1);
            }
        }
    }

    static CompositeState[] SubsetConstruction(Node[] nodes) {
        var compositeStates = new CompositeState[nodes.Length];

        for(int i = 0; i < nodes.Length; i++) {
            var compositeStateBuilder = new CompositeStateBuilder();
            
            // Explore lambda transitions
            DFSDo(nodes[i], (Node crnt) => {
                compositeStateBuilder.AddState(crnt.id);
            }, ((Node, char?) connection) => {
                return connection.Item2 == null;
            });

            compositeStates[i] = compositeStateBuilder.Build();
        }

        return compositeStates;
    }

    public static Automata<string, char> BuildFromRegex(string infixRegex) {
        Console.WriteLine($"Processing regex {infixRegex}");

        string postfixedRegex = RegexParser.GetPostfixedForm(infixRegex);

        Console.WriteLine($"Extracted postfixed notation: {postfixedRegex}");

        int nodesCount = 0;
        var automataToken = GetLambdaNFAToken(postfixedRegex, ref nodesCount);
        Console.WriteLine($"Calculated Lambda NFA Token:");
        PrintTokenHierarchy(automataToken);

        Node[] nodes = new Node[nodesCount];
        DFSDo(automataToken.startNode, (Node crnt) => {
            nodes[crnt.id] = crnt;
        }, _ => true);
        var compositeStates = SubsetConstruction(nodes);
        Console.WriteLine($"Subset construction:");
        for(int i = 0; i < nodesCount; i++) {
            Console.WriteLine($"<q{i}> = {compositeStates[i]}");
        }


        Console.WriteLine("\n\n");
        return null;
    }
}
