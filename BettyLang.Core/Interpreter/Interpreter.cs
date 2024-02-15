using BettyLang.Core.AST;

namespace BettyLang.Core.Interpreter
{
    public partial class Interpreter(Parser parser) : IStatementVisitor, IExpressionVisitor
    {
        private readonly Parser _parser = parser;
        private readonly Dictionary<string, FunctionDefinition> _functions = [];
        private readonly ScopeManager _scopeManager = new();
        private readonly InterpreterContext _context = new();

        public InterpreterResult Interpret()
        {
            var tree = _parser.Parse();
            return tree.Accept(this);
        }

        public InterpreterResult Visit(Program node)
        {
            // Visit each function definition and store it in a dictionary
            foreach (var function in node.Functions)
                function.Accept(this);

            if (_functions.TryGetValue("main", out var mainFunction))
            {
                if (mainFunction.Parameters.Count != 0)
                    throw new Exception("main() function cannot have parameters.");

                // Execute the main function
                return Visit(new FunctionCall("main", []));
            }
            
            throw new Exception("No main function found.");
        }

        public InterpreterResult Visit(ListValue node)
        {
            var elements = node.Elements.Select(e => e.Accept(this)).ToList();
            return InterpreterResult.FromList(elements);
        }

        public InterpreterResult Visit(ElementAccessExpression node)
        {
            var collection = node.List.Accept(this);
            var index = node.Index.Accept(this);

            if (index.Type != ResultType.Number || index.AsNumber() % 1 != 0)
            {
                throw new InvalidOperationException("Index for element access must be an integer.");
            }

            var indexValue = (int)index.AsNumber();

            switch (collection.Type)
            {
                case ResultType.String:
                    var stringValue = collection.AsString();
                    if (indexValue < 0 || indexValue >= stringValue.Length)
                        throw new IndexOutOfRangeException("String index out of range.");
                    return InterpreterResult.FromChar(stringValue[indexValue]);

                case ResultType.List:
                    var listValue = collection.AsList();
                    if (indexValue < 0 || indexValue >= listValue.Count)
                        throw new IndexOutOfRangeException("List index out of range.");
                    return listValue[indexValue];

                default:
                    throw new InvalidOperationException("Element access is supported only for lists and strings.");
            }
        }

        public InterpreterResult Visit(TernaryOperatorExpression node)
        {
            bool conditionResult = node.Condition.Accept(this).AsBoolean();
            return conditionResult ? node.TrueExpression.Accept(this) : node.FalseExpression.Accept(this);
        }

        public void Visit(ReturnStatement node)
        {
            if (node.ReturnValue != null)
            {
                _context.LastReturnValue = node.ReturnValue.Accept(this);
            }
            else
            {
                _context.LastReturnValue = InterpreterResult.None();
            }
            _context.FlowState = ControlFlowState.Return;
        }

        public void Visit(BreakStatement node)
        {
            if (!_context.IsInLoop)
            {
                throw new Exception("Break statement not inside a loop");
            }
            _context.FlowState = ControlFlowState.Break;
        }

        public void Visit(ContinueStatement node)
        {
            if (!_context.IsInLoop)
            {
                throw new Exception("Continue statement not inside a loop");
            }
            _context.FlowState = ControlFlowState.Continue;
        }

        public void Visit(DoWhileStatement node)
        {
            _context.IsInLoop = true;

            node.Body.Accept(this);

            while (node.Condition.Accept(this).AsBoolean())
            {
                node.Body.Accept(this);

                if (_context.FlowState == ControlFlowState.Continue)
                {
                    _context.FlowState = ControlFlowState.Normal;
                }
                else if (_context.FlowState == ControlFlowState.Break)
                {
                    _context.FlowState = ControlFlowState.Normal;
                    break;
                }
            }
        }

        public void Visit(ForStatement node)
        {
            // Execute the initializer once before the loop starts.
            node.Initializer?.Accept(this);

            _context.IsInLoop = true; // Set the loop state to true to allow continue and break statements.

            while (node.Condition == null || node.Condition.Accept(this).AsBoolean())
            {
                // Execute the body of the loop.
                node.Body.Accept(this);

                // If a continue statement was encountered, reset the flow state and skip directly to the increment.
                if (_context.FlowState == ControlFlowState.Continue)
                {
                    _context.FlowState = ControlFlowState.Normal; // Reset the flow state.
                }
                else if (_context.FlowState == ControlFlowState.Break)
                {
                    // If a break statement was encountered, exit the loop.
                    _context.FlowState = ControlFlowState.Normal; // Reset the flow state.
                    break;
                }

                // Execute the increment expression.
                node.Increment?.Accept(this);
            }

            _context.IsInLoop = false; // Ensure to reset the loop state after exiting.
        }

        public void Visit(WhileStatement node)
        {
            _context.IsInLoop = true;

            while (node.Condition.Accept(this).AsBoolean())
            {
                // Execute the body of the loop.
                node.Body.Accept(this);

                // Handle continue
                if (_context.FlowState == ControlFlowState.Continue)
                {
                    _context.FlowState = ControlFlowState.Normal; // Reset the flow state.
                }
                else if (_context.FlowState == ControlFlowState.Break)
                {
                    // If a break statement was encountered, exit the loop.
                    _context.FlowState = ControlFlowState.Normal; // Reset the flow state.
                    break;
                }
            }

            _context.IsInLoop = false;
        }

        public void Visit(IfStatement node)
        {
            var conditionResult = node.Condition.Accept(this).AsBoolean();

            if (conditionResult)
            {
                node.ThenStatement.Accept(this);
            }
            else
            {
                bool elseifExecuted = false;
                foreach (var (Condition, Statement) in node.ElseIfStatements)
                {
                    if (Condition.Accept(this).AsBoolean())
                    {
                        Statement.Accept(this);
                        elseifExecuted = true;
                        break; // Exit after the first true elseif to prevent executing multiple blocks
                    }
                }

                if (!elseifExecuted && node.ElseStatement != null)
                {
                    node.ElseStatement.Accept(this);
                }
            }
        }

        public InterpreterResult Visit(BinaryOperatorExpression node)
        {
            var leftResult = node.Left.Accept(this);
            var rightResult = node.Right.Accept(this);

            switch (node.Operator)
            {
                case TokenType.And:
                case TokenType.Or:
                    if (leftResult.Type != ResultType.Boolean || rightResult.Type != ResultType.Boolean)
                    {
                        throw new InvalidOperationException("Logical operations require both operands to be boolean.");
                    }
                    bool leftBoolean = leftResult.AsBoolean();
                    bool rightBoolean = rightResult.AsBoolean();
                    return InterpreterResult.FromBoolean(
                        node.Operator == TokenType.And ? leftBoolean && rightBoolean : leftBoolean || rightBoolean);

                case TokenType.Plus:
                    // If either operand is a string, concatenate as strings
                    if (leftResult.Type == ResultType.String || rightResult.Type == ResultType.String)
                    {
                        return InterpreterResult.FromString(leftResult.ToString() + rightResult.ToString());
                    }
                    // If the left operand is a list, add the right operand to the list
                    if (leftResult.Type == ResultType.List)
                    {
                        return InterpreterResult.AddToList(leftResult, rightResult);
                    }
                    // If the right operand is a list, add the left operand to the list
                    if (rightResult.Type == ResultType.List)
                    {
                        return InterpreterResult.AddToList(rightResult, leftResult);
                    }
                    // If none are strings or lists, perform numerical addition
                    return InterpreterResult.FromNumber(leftResult.AsNumber() + rightResult.AsNumber());
                case TokenType.Minus:
                case TokenType.Mul:
                case TokenType.Div:
                case TokenType.IntDiv:
                case TokenType.Caret:
                case TokenType.Mod:
                    return (leftResult.Type, rightResult.Type) switch 
                    {
                        (ResultType.Number or ResultType.Char, ResultType.Number or ResultType.Char) =>
                            PerformArithmeticOperation(leftResult, rightResult, node.Operator),
                        _ => throw new InvalidOperationException("Arithmetic operations require both operands to be numbers.")
                    };

                case TokenType.EqualEqual:
                case TokenType.NotEqual:
                case TokenType.LessThan:
                case TokenType.LessThanOrEqual:
                case TokenType.GreaterThan:
                case TokenType.GreaterThanOrEqual:
                    return PerformComparison(leftResult, rightResult, node.Operator);

                default:
                    throw new Exception($"Unsupported binary operator: {node.Operator} for operand types {leftResult.Type} and {rightResult.Type}");
            }
        }

        private static InterpreterResult PerformArithmeticOperation(InterpreterResult leftResult, InterpreterResult rightResult, TokenType operatorType)
        {
            double leftNumber = leftResult.AsNumber();
            double rightNumber = rightResult.AsNumber();

            return operatorType switch
            {
                TokenType.Plus => InterpreterResult.FromNumber(leftNumber + rightNumber),
                TokenType.Minus => InterpreterResult.FromNumber(leftNumber - rightNumber),
                TokenType.Mul => InterpreterResult.FromNumber(leftNumber * rightNumber),
                TokenType.Div => InterpreterResult.FromNumber(leftNumber / rightNumber),
                TokenType.IntDiv => InterpreterResult.FromNumber(Math.Floor(leftNumber / rightNumber)),
                TokenType.Caret => InterpreterResult.FromNumber(Math.Pow(leftNumber, rightNumber)),
                TokenType.Mod => InterpreterResult.FromNumber(leftNumber % rightNumber),
                _ => throw new Exception($"Unsupported arithmetic operator: {operatorType}")
            };
        }

        private static InterpreterResult PerformComparison(InterpreterResult leftResult, InterpreterResult rightResult, TokenType operatorType)
        {
            switch (leftResult.Type, rightResult.Type)
            {
                case (ResultType.Number or ResultType.Char, ResultType.Number or ResultType.Char):
                    double leftNumber = leftResult.AsNumber();
                    double rightNumber = rightResult.AsNumber();
                    return InterpreterResult.FromBoolean(operatorType switch
                    {
                        TokenType.EqualEqual => leftNumber == rightNumber,
                        TokenType.NotEqual => leftNumber != rightNumber,
                        TokenType.LessThan => leftNumber < rightNumber,
                        TokenType.LessThanOrEqual => leftNumber <= rightNumber,
                        TokenType.GreaterThan => leftNumber > rightNumber,
                        TokenType.GreaterThanOrEqual => leftNumber >= rightNumber,
                        _ => throw new Exception($"Unsupported operator for number comparison: {operatorType}")
                    });

                case (ResultType.String, ResultType.String):
                    string leftString = leftResult.AsString();
                    string rightString = rightResult.AsString();
                    return InterpreterResult.FromBoolean(operatorType switch
                    {
                        TokenType.EqualEqual => leftString == rightString,
                        TokenType.NotEqual => leftString != rightString,
                        _ => throw new Exception($"Unsupported operator for string comparison: {operatorType}")
                    });

                case (ResultType.Boolean, ResultType.Boolean):
                    bool leftBoolean = leftResult.AsBoolean();
                    bool rightBoolean = rightResult.AsBoolean();
                    return InterpreterResult.FromBoolean(operatorType switch
                    {
                        TokenType.EqualEqual => leftBoolean == rightBoolean,
                        TokenType.NotEqual => leftBoolean != rightBoolean,
                        _ => throw new Exception($"Unsupported operator for boolean comparison: {operatorType}")
                    });

                default:
                    throw new Exception("Type mismatch or unsupported types for comparison.");
            }
        }

        public InterpreterResult Visit(BooleanLiteral node) => InterpreterResult.FromBoolean(node.Value);
        public InterpreterResult Visit(NumberLiteral node) => InterpreterResult.FromNumber(node.Value);
        public InterpreterResult Visit(StringLiteral node) => InterpreterResult.FromString(node.Value);
        public InterpreterResult Visit(CharLiteral node) => InterpreterResult.FromChar(node.Value);

        public void Visit(CompoundStatement node)
        {
            foreach (var statement in node.Statements)
            {
                statement.Accept(this);

                // Handle control flow changes
                if (_context.FlowState != ControlFlowState.Normal)
                {
                    break; // Exit the compound statement early
                }
            }
        }

        private static InterpreterResult ApplyCompoundOperation(InterpreterResult left, InterpreterResult right, TokenType operatorType)
        {
            switch (left.Type, right.Type)
            {
                case (ResultType.Number or ResultType.Char, ResultType.Number or ResultType.Char):
                    operatorType = operatorType switch
                    {
                        TokenType.PlusEqual => TokenType.Plus,
                        TokenType.MinusEqual => TokenType.Minus,
                        TokenType.MulEqual => TokenType.Mul,
                        TokenType.DivEqual => TokenType.Div,
                        TokenType.IntDivEqual => TokenType.IntDiv,
                        TokenType.CaretEqual => TokenType.Caret,
                        TokenType.ModEqual => TokenType.Mod,
                        _ => throw new InvalidOperationException("Unsupported compound assignment operator.")
                    };
                    return PerformArithmeticOperation(left, right, operatorType);

                case (ResultType.String, _):
                    if (operatorType != TokenType.PlusEqual)
                        throw new InvalidOperationException("Compound assignment for strings only supports the += operator.");
                    return InterpreterResult.FromString(left.AsString() + right.ToString());

                case (ResultType.List, _):
                    if (operatorType != TokenType.PlusEqual)
                        throw new InvalidOperationException("Compound assignment for lists only supports the += operator.");
                    return InterpreterResult.AddToList(left, right);

                default:
                    throw new InvalidOperationException("Compound assignment is not supported for the given types.");
            }
        }

        public InterpreterResult Visit(AssignmentExpression node)
        {
            // Evaluate RHS
            InterpreterResult rhsValue = node.Right.Accept(this);

            // Simple variable assignment
            if (node.Left is Variable variableNode)
            {
                string variableName = variableNode.Name;

                // If compound assignment, get current value
                if (node.OperatorType != TokenType.Equal)
                {
                    var lhsValue = _scopeManager.LookupVariable(variableName);
                    rhsValue = ApplyCompoundOperation(lhsValue, rhsValue, node.OperatorType);
                }

                // Perform the assignment
                _scopeManager.SetVariable(variableName, rhsValue);
                return rhsValue;
            }
            // Assignment to a list element
            else if (node.Left is ElementAccessExpression elementAccess)
            {
                InterpreterResult listValue = elementAccess.List.Accept(this);
                InterpreterResult indexValue = elementAccess.Index.Accept(this);

                if (listValue.Type != ResultType.List || indexValue.Type != ResultType.Number)
                    throw new InvalidOperationException("Invalid element access or index type.");

                List<InterpreterResult> list = listValue.AsList();
                int index = Convert.ToInt32(indexValue.AsNumber());

                // Ensure index is within bounds
                if (index < 0 || index >= list.Count)
                    throw new IndexOutOfRangeException("Index out of range for list assignment.");

                // If compound assignment, apply the operator to the current element
                if (node.OperatorType != TokenType.Equal)
                {
                    rhsValue = ApplyCompoundOperation(list[index], rhsValue, node.OperatorType);
                }

                // Update the list element
                list[index] = rhsValue;
                return rhsValue;
            }
            else
            {
                throw new InvalidOperationException("The left-hand side of an assignment must be a variable or list element.");
            }
        }

        public InterpreterResult Visit(Variable node) => _scopeManager.LookupVariable(node.Name);

        public void Visit(EmptyStatement node) { }

        public InterpreterResult Visit(FunctionCall node)
        {
            // If the function is an intrinsic function, invoke it
            if (_intrinsicFunctions.TryGetValue(node.FunctionName, out var intrinsicFunction))
                return intrinsicFunction.Invoke(node, this);

            // Function is not an intrinsic function, look for a user-defined function
            if (!_functions.TryGetValue(node.FunctionName, out var function))
                throw new Exception($"Function {node.FunctionName} is not defined.");

            // Enter a new scope for function execution
            _scopeManager.EnterScope();

            // Prepare for function execution
            bool previousLoopContext = _context.IsInLoop;
            _context.IsInLoop = false; // Reset loop context
            var previousFlowState = _context.FlowState; // Save the current flow state
            _context.FlowState = ControlFlowState.Normal; // Reset flow for function execution

            // Set function parameters in the new scope
            for (int i = 0; i < node.Arguments.Count; i++)
            {
                var argValue = node.Arguments[i].Accept(this);
                _scopeManager.SetVariable(function.Parameters[i], argValue);
            }

            // Execute function body
            function.Body.Accept(this);

            // Capture the return value if the function execution resulted in a return
            InterpreterResult returnValue = _context.FlowState == ControlFlowState.Return ? _context.LastReturnValue : InterpreterResult.None();

            // Restore the context after function execution
            _context.IsInLoop = previousLoopContext;
            _context.FlowState = previousFlowState;

            // Exit the function scope
            _scopeManager.ExitScope();

            return returnValue;
        }

        public void Visit(FunctionDefinition node)
        {
            if (_intrinsicFunctions.ContainsKey(node.FunctionName))
                throw new Exception($"Function name '{node.FunctionName}' is reserved for built-in functions.");

            _functions[node.FunctionName] = node;
        }

        public InterpreterResult Visit(UnaryOperatorExpression node)
        {
            var operandResult = node.Operand.Accept(this);
            TokenType op = node.Operator;
            OperatorFixity fixity = node.Fixity;

            switch (op, fixity)
            {
                case (TokenType.Plus, OperatorFixity.Prefix):
                    return operandResult;
                case (TokenType.Minus, OperatorFixity.Prefix):
                    return InterpreterResult.FromNumber(-operandResult.AsNumber());
                case (TokenType.Not, OperatorFixity.Prefix):
                    return InterpreterResult.FromBoolean(!operandResult.AsBoolean());

                case (TokenType.Increment or TokenType.Decrement, _):
                    if (node.Operand is Variable variableNode)
                    {
                        if (operandResult.Type != ResultType.Number && operandResult.Type != ResultType.Char)
                            throw new InvalidOperationException(
                                $"{fixity} {op} operators can only be applied to numbers or characters.");

                        var variableName = variableNode.Name;
                        var currentValue = operandResult.AsNumber();
                        var newValue = op switch
                        {
                            TokenType.Increment => currentValue + 1,
                            TokenType.Decrement => currentValue - 1,
                            _ => throw new InvalidOperationException($"Unsupported {fixity} assignment operator {op}")
                        };
                        _scopeManager.SetVariable(variableName, InterpreterResult.FromNumber(newValue));

                        return node.Fixity == OperatorFixity.Prefix ?
                            InterpreterResult.FromNumber(newValue) : InterpreterResult.FromNumber(currentValue);
                    }

                    // Check if the operand is an element access in a list
                    if (node.Operand is ElementAccessExpression elementAccess)
                    {
                        // Ensure the list and index are valid
                        var listResult = elementAccess.List.Accept(this);
                        var indexResult = elementAccess.Index.Accept(this);
                        if (listResult.Type != ResultType.List || indexResult.Type != ResultType.Number)
                            throw new InvalidOperationException("Invalid element access in list.");

                        var list = listResult.AsList();
                        var index = (int)indexResult.AsNumber();
                        if (index < 0 || index >= list.Count)
                            throw new IndexOutOfRangeException("List index out of range.");

                        // Ensure the target element is a number or a character
                        if (list[index].Type != ResultType.Number 
                            && list[index].Type != ResultType.Char)
                            throw new InvalidOperationException(
                                $"{fixity} {op} operators can only be applied to numbers or characters.");

                        var currentValue = list[index].AsNumber();
                        var newValue = op switch
                        {
                            TokenType.Increment => currentValue + 1,
                            TokenType.Decrement => currentValue - 1,
                            _ => throw new InvalidOperationException($"Unsupported {fixity} assignment operator {op}")
                        };

                        // Update the list element
                        list[index] = InterpreterResult.FromNumber(newValue);

                        // Depending on whether it's prefix or postfix, return the new or old value
                        return node.Fixity == OperatorFixity.Prefix ? 
                            InterpreterResult.FromNumber(newValue) : InterpreterResult.FromNumber(currentValue);
                    }

                    throw new Exception($"The operand of a {fixity} {op} operator must be a variable or a list element.");


                default:
                    throw new InvalidOperationException($"Unsupported {fixity} operator {op}");
            }
        }

        public void Visit(ExpressionStatement node) => node.Expression.Accept(this);
    }
}