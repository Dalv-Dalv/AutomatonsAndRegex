struct Production {
    public readonly string str;
    public readonly bool isTerminating;
    public readonly int terminalsCount;

    public Production(string str, bool isTerminating) {
        this.str = str;
        this.isTerminating = isTerminating;
        terminalsCount = str.Count((x) => !(x >= 'A' && x <= 'Z'));
    }

    public override string ToString() {
        return $"{str.ToString()} [{isTerminating}]";
    }
}
