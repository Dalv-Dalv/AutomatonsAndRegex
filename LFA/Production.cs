struct Production {
    public readonly string str;
    public readonly bool isTerminating;

    public Production(string str, bool isTerminating) {
        this.str = str;
        this.isTerminating = isTerminating;
    }

    public override string ToString() {
        return $"{str.ToString()} [{isTerminating}]";
    }
}
