// Ghita Vladut
// Grupa 151
// C# .NET 8.0


class CompositeStateBuilder {
    readonly List<AutomatonNode> states;
    public CompositeStateBuilder() {
        states = new();
    }

    public void AddState(AutomatonNode state) {
        states.Add(state);
    }
    public void AddState(CompositeNode compositeNode) {
        foreach(var node in compositeNode) {
            states.Add(node);
        }
    }

    public CompositeNode Build() {
        return new CompositeNode(states);
    }
}