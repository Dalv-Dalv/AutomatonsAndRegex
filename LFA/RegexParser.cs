using System;
using System.Text;

public static class RegexParser {
    static Dictionary<char, int> operatorPrecedence = new(){
        {'?', 3},
        {'*', 3},
        {'+', 3},
        {'.', 2},
        {'|', 1}
    };

    static bool IsSymbol(char c) {
        return !"+|*()?.".Contains(c);
    }

    static string GetExplicitConcatenationForm(string expression) {
        StringBuilder sb = new StringBuilder();
        const string unaryOperators = "*?+";

        for(int i = 0; i < expression.Count() - 1; i++) {
            sb.Append(expression[i]);

            if((IsSymbol(expression[i]) && IsSymbol(expression[i + 1]))
            || (IsSymbol(expression[i]) && expression[i + 1] == '(')
            || (expression[i] == ')' && IsSymbol(expression[i + 1]))
            || (expression[i] == ')' && expression[i + 1] == '(')
            || (unaryOperators.Contains(expression[i]) && (IsSymbol(expression[i + 1]) || expression[i + 1] == '('))) {
                sb.Append('.');
            
                continue;
            }
        }

        sb.Append(expression[expression.Length - 1]);

        return sb.ToString();
    }

    public static string GetPostfixedForm(string expression) {
        expression = GetExplicitConcatenationForm(expression);

        Console.WriteLine($"Explicit concatenation form: {expression}");

        StringBuilder sb = new();
        Stack<(char, int)> operatorStack = new();

        for(int i = 0; i < expression.Length; i++) {
            if(IsSymbol(expression[i])) {
                sb.Append(expression[i]);
                continue;
            }

            if(expression[i] == '(') {
                operatorStack.Push(('(', 0));
                continue;
            }

            if(expression[i] == ')') {
                while(operatorStack.Peek().Item1 != '(') {
                    sb.Append(operatorStack.Pop().Item1);
                }
                operatorStack.Pop();
                continue;
            }

            int precedence = operatorPrecedence[expression[i]];

            while(operatorStack.Count > 0 && operatorStack.Peek().Item2 > precedence) {
                sb.Append(operatorStack.Pop().Item1);
            }
            operatorStack.Push((expression[i], precedence));
        }
        
        while(operatorStack.Count > 0) {
            sb.Append(operatorStack.Pop().Item1);
        }

        return sb.ToString();
    }
}
