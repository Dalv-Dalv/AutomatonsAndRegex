// Ghita Vladut
// Grupa 151
// C# .NET 8.0

public class AutomatonNode : IComparable {
    public int id;
    public List<(AutomatonNode, char?)> connections;
    public AutomatonNode(int id) {
        this.id = id;
        connections = new List<(AutomatonNode, char?)>();
    }

    public void AddConnection(AutomatonNode node, char? connection) {
        connections.Add((node, connection));
    }

    public int CompareTo(object? obj) {
        return id.CompareTo((obj as AutomatonNode).id);
    }

    public override string ToString() {
        return $"({id})";
    }
}
