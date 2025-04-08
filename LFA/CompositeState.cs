// Ghita Vladut
// Grupa 151
// C# .NET 8.0


class CompositeState {
    readonly SortedSet<int> states;

    public CompositeState(IEnumerable<int> states) {
        this.states = new SortedSet<int>(states);
    }

    public IEnumerable<int> Iterator => states;
    public bool Contains(int state) => states.Contains(state);
    public CompositeState UnionWith(CompositeState other) {
        return new CompositeState(states.Union(other.states));
    }

    public override bool Equals(object? obj) {
        return obj is CompositeState other && states.Equals(other.states);
    }
    public override int GetHashCode() {
        unchecked {
            int hash = 17;
            foreach(int state in states) {
                hash = hash * 31 + state.GetHashCode();
            }   
            return hash;
        }
    }
    public override string ToString() {
        return "q" + string.Join("", states);
    }
}

class CompositeStateBuilder {
    readonly List<int> states;
    public CompositeStateBuilder() {
        states = new List<int>();
    }

    public void AddState(int state) {
        states.Add(state);
    }

    public CompositeState Build() {
        return new CompositeState(states);
    }
}