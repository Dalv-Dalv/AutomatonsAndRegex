class CFGBuilder {
    List<char> terminals = new(), variables = new();
    HashSet<char> terminalsSet = new(), varSet = new();
    Dictionary<char, List<Production>> productions = new(); 

    char? startSymbol = null;

    bool IsNonterminal(char c) {
        return c >= 'A' && c <= 'Z';
    }

    public void AddProduction(string production) {
        production = production.Replace(" ", "");
        production = production.Replace('_', ' ');
        // S->aSb|#

        char nt = production[0];
        if(startSymbol == null) {
            startSymbol = nt;
        }

        if(!varSet.Contains(nt)) {
            productions[nt] = new List<Production>();
            varSet.Add(nt);
        }


        var prods = production.Substring(production.IndexOf("->") + 2).Split('|');
        foreach(var prod in prods) {
            bool terminating = true;
            for(int i = 0; i < prod.Length; i++) {
                if(IsNonterminal(prod[i])) {
                    terminating = false;

                    if(varSet.Contains(prod[i])) continue;

                    varSet.Add(prod[i]);
                    variables.Add(prod[i]);
                } else {
                    if(terminalsSet.Contains(prod[i])) continue;

                    terminalsSet.Add(prod[i]);
                    terminals.Add(prod[i]);
                }
            }

            var x = new Production(prod.Replace("#", ""), terminating);
            if(!productions.ContainsKey(nt)) {
                productions[nt] = new List<Production>();
            }
            productions[nt].Add(x);
        }
    }

    public CFG Build() {
        if(startSymbol == null) throw new Exception("CFG has no productions");

        var dict = productions.ToDictionary(pair => pair.Key, pair => pair.Value.ToArray());

        return new CFG(startSymbol.Value, terminals.ToArray(), variables.ToArray(), dict);
    }
}