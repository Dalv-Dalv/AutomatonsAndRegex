using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class CFG {
    char startSymbol;
    char[] terminals, nonterminals;
    Dictionary<char, Production[]> productions;
    Random rand;

    public CFG(char startSymbol, char[] terminals, char[] nonterminals, Dictionary<char, Production[]> productions) {
        this.startSymbol = startSymbol;
        this.terminals = terminals;
        this.nonterminals = nonterminals;
        this.productions = productions;

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
        public List<(char, int)> productionsUsed;
        public List<string> derivationSteps;

        public DerivationNode(string str) {
            this.str = str;
            productionsUsed = new List<(char, int)>();
            derivationSteps = new List<string>();
        }

        public DerivationNode(string str, List<(char, int)> productionsUsed, List<string> derivationSteps) {
            this.str = str;
            this.productionsUsed = productionsUsed;
            this.derivationSteps = derivationSteps;
        }
    }
    public string? DerivateWord(string word) {
        Queue<DerivationNode> queue = new();
        queue.Enqueue(new DerivationNode(
            $"{startSymbol}",
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
        Queue<string> queue = new();
        queue.Enqueue($"{startSymbol}");

        HashSet<string> visited = new();
        visited.Add($"{startSymbol}");

        while(queue.Count > 0) {
            var node = queue.Dequeue();

            if(node == word) {
                return true;
            }

            int? ntI = null;
            bool valid = true;
            for(int i = 0; i < node.Length; i++) {
                char c = node[i];
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

            char symbol = node[ntI.Value];
            string prefix = node.Substring(0, ntI.Value);
            string suffix = node.Substring(ntI.Value + 1);
            for(int i = 0; i < productions[symbol].Length; i++) {
                string newStr = prefix + productions[symbol][i].str + suffix;

                if(visited.Contains(newStr)) continue;
                visited.Add(newStr);

                queue.Enqueue(newStr);
            }
        }

        return false;
    }
}
