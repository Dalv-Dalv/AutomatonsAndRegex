// Ghita Vladut
// Grupa 151
// C# .NET 8.0


using System.Collections;

struct CompositeNode : IEnumerable<AutomatonNode> {
    readonly SortedSet<AutomatonNode> states;

    public CompositeNode() {
        states = new SortedSet<AutomatonNode>();
    }

    public CompositeNode(IEnumerable<AutomatonNode> states) {
        this.states = new(states);
    }

    public IEnumerable<AutomatonNode> Iterator => states;
    public bool Contains(AutomatonNode state) => states.Contains(state);

    public CompositeNode UnionWith(CompositeNode other) {
        return new CompositeNode(states.Union(other.states));
    }


    public CompositeNode StepWith(char? letter) {
        var builder = new CompositeStateBuilder();

        foreach(var state in states) {
            foreach(var edge in state.connections) {
                if(edge.Item2 == letter) builder.AddState(edge.Item1);
            }
        }

        return builder.Build();
    }

    public bool IsEmpty() => states.Count == 0;

    public override bool Equals(object? obj) {
        return obj is CompositeNode other && states.SetEquals(other.states);
    }
    public override int GetHashCode() {
        unchecked {
            int hash = 17;
            foreach(var state in states) {
                hash = hash * 31 + state.GetHashCode();
            }   
            return hash;
        }
    }
    public override string ToString() {
        return "{" + string.Join(",", states) + "}";
    }

    public IEnumerator<AutomatonNode> GetEnumerator() {
        return ((IEnumerable<AutomatonNode>)states).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return ((IEnumerable)states).GetEnumerator();
    }
}
