using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class CFG {
    char startSymbol;
    HashSet<char> terminals, nonterminals;
    Dictionary<char, Production[]> productions;
    Random rand;
    int maxTC;
    public CFG(char startSymbol, HashSet<char> terminals, HashSet<char> nonterminals, Dictionary<char, Production[]> productions) {
        this.startSymbol = startSymbol;
        this.terminals = terminals;
        this.nonterminals = nonterminals;
        this.productions = productions;

        foreach(var prods in productions.Values) {
            foreach(var production in prods) {
                maxTC = int.Max(maxTC, production.terminalsCount);
            }
        }

        rand = new Random();
    }

    bool RandBool(int probability) {
        return rand.Next(probability) == 0;
    }

    bool IsTerminal(char c) {
        return !(c >= 'A' && c <= 'Z');
    }

    bool GenerateRandomWord(int maxDepth, ref int maxLength, char symbol, ref StringBuilder sb) {
        if(maxDepth < 0 || sb.Length > maxLength) return false;

        int[] indices = Enumerable.Range(0, productions[symbol].Length).ToArray();
        rand.Shuffle(indices);

        for(int i = 0; i < productions[symbol].Length; i++) {
            var prod = productions[symbol][indices[i]];

            if(prod.isTerminating) {
                sb.Append(prod.str);
                return true;
            }

            bool succesful = true;
            StringBuilder temp = new();
            for(int c = 0; c < prod.str.Length; c++) {
                if(IsTerminal(prod.str[c])) {
                    temp.Append(prod.str[c]);
                    continue;
                }

                if(!GenerateRandomWord(maxDepth - 1, ref maxLength, prod.str[c], ref temp)) {
                    succesful = false;
                    break;
                }
            }

            if(!succesful) continue;

            sb.Append(temp);
            return true;
        }

        return false;
    }

    public string GenerateRandomWord(int maxDepth, int maxLength) {
        StringBuilder sb = new();
        while(!GenerateRandomWord(maxDepth, ref maxLength, startSymbol, ref sb)) {
            sb.Clear();
        }
        return sb.ToString();
    }

    struct DerivationNode {
        public string str;
        public int terminalsCount;
        public List<(char, int)> productionsUsed;
        public List<string> derivationSteps;

        public DerivationNode(string str, int terminalsCount) {
            this.str = str;
            this.terminalsCount = terminalsCount;
            productionsUsed = new List<(char, int)>();
            derivationSteps = new List<string>();
        }

        public DerivationNode(string str, int terminalsCount, List<(char, int)> productionsUsed, List<string> derivationSteps) {
            this.str = str;
            this.terminalsCount = terminalsCount;
            this.productionsUsed = productionsUsed;
            this.derivationSteps = derivationSteps;
        }
    }
    public string? DerivateWord(string word) {
        if(!word.All(c => terminals.Contains(c))) return null;

        Queue<DerivationNode> queue = new();
        queue.Enqueue(new DerivationNode(
            $"{startSymbol}", 0,
            new List<(char, int)>(),
            new List<string>()
        ));
        queue.First().derivationSteps.Add($"{startSymbol}");

        HashSet<string> visited = new();
        visited.Add($"{startSymbol}");

        DerivationNode? finalDerivation = null;
        while(queue.Count > 0) {
            var node = queue.Dequeue();

            if(node.str == word) {
                finalDerivation = node;
                break;
            }

            if(node.terminalsCount > word.Length) continue;
            if(node.str.Length > maxTC * word.Length + 1) continue;

            int? ntI = null;
            bool valid = true;
            for(int i = 0; i < node.str.Length; i++) {
                char c = node.str[i];
                if(IsTerminal(c)) {
                    if(i >= word.Length || c != word[i]) {
                        valid = false;
                        break;
                    }
                    continue;
                }

                ntI = i;
                break;
            }

            if(!valid) continue;
            if(ntI == null) continue;

            char symbol = node.str[ntI.Value];
            string prefix = node.str.Substring(0, ntI.Value);
            string suffix = node.str.Substring(ntI.Value + 1);
            for(int i = 0; i < productions[symbol].Length; i++) {
                string newStr = prefix + productions[symbol][i].str + suffix;

                if(visited.Contains(newStr)) continue;
                visited.Add(newStr);

                queue.Enqueue(new DerivationNode(
                    newStr,
                    node.terminalsCount + productions[symbol][i].terminalsCount,
                    node.productionsUsed.Append((symbol, i)).ToList(),
                    node.derivationSteps.Append(newStr).ToList()
                ));
            }
        }

        if(finalDerivation == null) return null;

        var deriv = finalDerivation.Value;

        StringBuilder sb = new();
        for(int i = 0; i < deriv.productionsUsed.Count; i++) {
            sb.Append($"\"{deriv.derivationSteps[i]}\" => \"{deriv.derivationSteps[i + 1]}\"  | Used derivation: {deriv.productionsUsed[i].Item1} -> {productions[deriv.productionsUsed[i].Item1][deriv.productionsUsed[i].Item2].str}\n");
        }
        return sb.ToString();
    }

    public bool ContainsWord(string word) {
        if(!word.All(c => terminals.Contains(c))) return false;

        Queue<(string, int)> queue = new();
        queue.Enqueue(($"{startSymbol}", 0));

        HashSet<string> visited = new();
        visited.Add($"{startSymbol}");

        while(queue.Count > 0) {
            var node = queue.Dequeue();

            if(node.Item1 == word) {
                return true;
            }

            if(node.Item2 > word.Length) continue;
            if(node.Item1.Length > maxTC * word.Length + 1) continue;

            int? ntI = null;
            bool valid = true;
            for(int i = 0; i < node.Item1.Length; i++) {
                char c = node.Item1[i];
                if(IsTerminal(c)) {
                    if(i >= word.Length || c != word[i]) {
                        valid = false;
                        break;
                    }
                    continue;
                }

                ntI = i;
                break;
            }

            if(!valid) continue;
            if(ntI == null) continue;

            char symbol = node.Item1[ntI.Value];
            string prefix = node.Item1.Substring(0, ntI.Value);
            string suffix = node.Item1.Substring(ntI.Value + 1);
            for(int i = 0; i < productions[symbol].Length; i++) {
                string newStr = prefix + productions[symbol][i].str + suffix;

                if(node.Item2 + productions[symbol][i].terminalsCount > word.Length) continue;
                if(visited.Contains(newStr)) continue;
                visited.Add(newStr);

                queue.Enqueue((newStr, node.Item2 + productions[symbol][i].terminalsCount));
            }
        }

        return false;
    }
}
