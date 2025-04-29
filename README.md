# Automatons and Regular Expressions (Regex)
This project was made for a university course about Formal Languages.

The implementation can handle deterministic and nondeterministic finite automatons. It can handle queries in standard DFAs and NFAs but it cant querry into Lambda-NFAs.

Regex parsing relies on these steps:
1. Converting the expression into its equivalent post-fixed notation
2. Tokenizing the post-fixed expression into a graph representation
3. While tokenizing, perform the operations and build the lambda-NFA
4. Lambda closure
5. Subset construction

To parse a regex, `AutomataBuilder.BuildFromRegex(regexString);` is called and a deterministic `Automata<string, char>` is returned.
Behind the scenes, this function first explicitizes concatenations such that an expression `(a|bc)d` is explicitized to `(a|b.c).d` where the `.` represents the concatenation operation.
After that's done, the expression is turned into its postfixed notation using the Shunting Yard algorithm, the previous expression becomes `abc.|`
