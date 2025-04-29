# Automatons and Regular Expressions (Regex)
This project was made for a university course about Formal Languages.

The implementation can handle deterministic and nondeterministic finite automatons. It can handle queries in standard DFAs and NFAs but it cant querry into Lambda-NFAs.

Regex parsing relies on these steps:
1. Converting the expression into its equivalent post-fixed notation
2. Tokenizing the post-fixed expression into a graph representation
3. While tokenizing, perform the operations and build the lambda-NFA
4. Lambda closure
5. Subset construction

To parse a regex, `AutomataBuilder.BuildFromRegex(regexString);` is called and a deterministic `Automata<string, char>` is returned. <br>
That function has these following steps:
1. Explicitizes concatenations such that an expression `(a|bc)d` is explicitized to `(a|b.c).d` where the `.` represents the concatenation operation
2. The expression is turned into its postfixed notation using the Shunting Yard algorithm, the previous expression becomes `abc.|`
3. The alphabet present in the regex is extracted, in this case `{a, b, c, d}`
4. A 'token' is created based on the postfixed regex representing transitions in the equivalent lambda-NFA, which is constructed based on thompsons algorithm. The token holds just one start and final state to make working with it easier. A token hold a start node and a final node. The construction of the lambda-NFA through this token is done while parsing the postfixed regex using a stack of tokens:
  - If a character from the alphabet is read, a token is created which holds the equivalent NFA, in our example this would first be: `(0)---a--->(1)`, our stack at this point is `{[0-a->1]}`. If we repeat this step until we reach an operator our stack looks like `{[0-a->1], [2-b->3], [4-c->5]}`
  - If an operator is read, pop the number of operands it needs from the stack and construct a new token that depends on the operator, so our previous stack would now look like `{[0-a->1], [2-b->3-L->4-c->5]}` where L is a lambda transition.
  - This operation is repeated until we reach the end of the postfixed regex, and we should end up with a single token on our stack representing the equivalent lambda-NFA.
5. The nodes, which were created sequentially using a global id which was incremented after every new node, are then stored in an array.
6. We perform lambda closure on the states and we will end up with composite states which hold multiple states from the initial array, so for example we could have a composite node `A = {q0, q1, q4}`
7. Subset construction using those composite nodes.
8. Bring it all together, turn the data into something to use when creating the final DFA in the form of a `Automata<string, char>`
9. All done!
