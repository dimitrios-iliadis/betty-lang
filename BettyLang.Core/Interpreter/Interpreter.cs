using BettyLang.Core.AST;

namespace BettyLang.Core.Interpreter
{
    public partial class Interpreter(Parser parser) : IStatementVisitor, IExpressionVisitor
    {
        private readonly Parser _parser = parser;
        private readonly Dictionary<string, FunctionDefinition> _functions = [];
        private readonly ScopeManager _scopeManager = new();
        private readonly InterpreterContext _context = new();

        public Value Interpret()
        {
            var tree = _parser.Parse();
            return tree.Accept(this);
        }

        public Value Visit(Program node)
        {
            // Visit each global variable declaration and store it in the global scope
            foreach (var global in node.Globals)
                _scopeManager.DeclareGlobal(global, Value.None()); // Initialize to None

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

        public Value Visit(ListValue node)
        {
            var elements = node.Elements.Select(e => e.Accept(this)).ToList();
            return Value.FromList(elements);
        }

        public Value Visit(IndexerExpression node)
        {
            var collection = node.Collection.Accept(this);
            var index = node.Index.Accept(this);

            if (index.Type != ValueType.Number || index.AsNumber() % 1 != 0)
            {
                throw new InvalidOperationException("Index for element access must be an integer.");
            }

            var indexValue = (int)index.AsNumber();

            switch (collection.Type)
            {
                case ValueType.String:
                    var stringValue = collection.AsString();
                    if (indexValue < 0 || indexValue >= stringValue.Length)
                        throw new IndexOutOfRangeException("String index out of range.");
                    return Value.FromChar(stringValue[indexValue]);

                case ValueType.List:
                    var listValue = collection.AsList();
                    if (indexValue < 0 || indexValue >= listValue.Count)
                        throw new IndexOutOfRangeException("List index out of range.");
                    return listValue[indexValue];

                default:
                    throw new InvalidOperationException("Indexing is supported only for lists and strings.");
            }
        }

        public Value Visit(TernaryOperatorExpression node)
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
                _context.LastReturnValue = Value.None();
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
            _context.EnterLoop();

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

            _context.ExitLoop();
        }

        public void Visit(ForStatement node)
        {
            // Execute the initializer once before the loop starts.
            node.Initializer?.Accept(this);

            _context.EnterLoop(); // Enter a new loop context.

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

            _context.ExitLoop(); // Exit the loop context.
        }

        public void Visit(WhileStatement node)
        {
            _context.EnterLoop();

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

            _context.ExitLoop();
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

        public Value Visit(BinaryOperatorExpression node)
        {
            var leftResult = node.Left.Accept(this);
            var rightResult = node.Right.Accept(this);

            switch (node.Operator)
            {
                case TokenType.And:
                case TokenType.Or:
                    if (leftResult.Type != ValueType.Boolean || rightResult.Type != ValueType.Boolean)
                    {
                        throw new InvalidOperationException("Logical operations require both operands to be boolean.");
                    }
                    bool leftBoolean = leftResult.AsBoolean();
                    bool rightBoolean = rightResult.AsBoolean();
                    return Value.FromBoolean(
                        node.Operator == TokenType.And ? leftBoolean && rightBoolean : leftBoolean || rightBoolean);

                case TokenType.Plus:
                    // If either operand is a string, concatenate as strings
                    if (leftResult.Type == ValueType.String || rightResult.Type == ValueType.String)
                    {
                        return Value.FromString(leftResult.ToString() + rightResult.ToString());
                    }
                    // If the left operand is a list, add the right operand to the list
                    if (leftResult.Type == ValueType.List)
                    {
                        return Value.AddToList(leftResult, rightResult);
                    }
                    // If the right operand is a list, add the left operand to the list
                    if (rightResult.Type == ValueType.List)
                    {
                        return Value.AddToList(rightResult, leftResult);
                    }
                    // If none are strings or lists, perform numerical addition
                    return Value.FromNumber(leftResult.AsNumber() + rightResult.AsNumber());
                case TokenType.Minus:
                case TokenType.Mul:
                case TokenType.Div:
                case TokenType.IntDiv:
                case TokenType.Caret:
                case TokenType.Mod:
                    return (leftResult.Type, rightResult.Type) switch 
                    {
                        (ValueType.Number or ValueType.Char, ValueType.Number or ValueType.Char) =>
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

        private static Value PerformArithmeticOperation(Value leftResult, Value rightResult, TokenType operatorType)
        {
            double leftNumber = leftResult.AsNumber();
            double rightNumber = rightResult.AsNumber();

            return operatorType switch
            {
                TokenType.Plus => Value.FromNumber(leftNumber + rightNumber),
                TokenType.Minus => Value.FromNumber(leftNumber - rightNumber),
                TokenType.Mul => Value.FromNumber(leftNumber * rightNumber),
                TokenType.Div => Value.FromNumber(leftNumber / rightNumber),
                TokenType.IntDiv => Value.FromNumber(Math.Floor(leftNumber / rightNumber)),
                TokenType.Caret => Value.FromNumber(Math.Pow(leftNumber, rightNumber)),
                TokenType.Mod => Value.FromNumber(leftNumber % rightNumber),
                _ => throw new Exception($"Unsupported arithmetic operator: {operatorType}")
            };
        }

        private static Value PerformComparison(Value leftResult, Value rightResult, TokenType operatorType)
        {
            switch (leftResult.Type, rightResult.Type)
            {
                case (ValueType.Number or ValueType.Char, ValueType.Number or ValueType.Char):
                    double leftNumber = leftResult.AsNumber();
                    double rightNumber = rightResult.AsNumber();
                    return Value.FromBoolean(operatorType switch
                    {
                        TokenType.EqualEqual => leftNumber == rightNumber,
                        TokenType.NotEqual => leftNumber != rightNumber,
                        TokenType.LessThan => leftNumber < rightNumber,
                        TokenType.LessThanOrEqual => leftNumber <= rightNumber,
                        TokenType.GreaterThan => leftNumber > rightNumber,
                        TokenType.GreaterThanOrEqual => leftNumber >= rightNumber,
                        _ => throw new Exception($"Unsupported operator for number comparison: {operatorType}")
                    });

                case (ValueType.String, ValueType.String):
                    string leftString = leftResult.AsString();
                    string rightString = rightResult.AsString();
                    return Value.FromBoolean(operatorType switch
                    {
                        TokenType.EqualEqual => leftString == rightString,
                        TokenType.NotEqual => leftString != rightString,
                        _ => throw new Exception($"Unsupported operator for string comparison: {operatorType}")
                    });

                case (ValueType.Boolean, ValueType.Boolean):
                    bool leftBoolean = leftResult.AsBoolean();
                    bool rightBoolean = rightResult.AsBoolean();
                    return Value.FromBoolean(operatorType switch
                    {
                        TokenType.EqualEqual => leftBoolean == rightBoolean,
                        TokenType.NotEqual => leftBoolean != rightBoolean,
                        _ => throw new Exception($"Unsupported operator for boolean comparison: {operatorType}")
                    });

                case (ValueType.List, ValueType.List) when operatorType == TokenType.EqualEqual || operatorType == TokenType.NotEqual:
                    var leftList = leftResult.AsList();
                    var rightList = rightResult.AsList();

                    // Short-circuit evaluation for lists of different lengths
                    if (leftList.Count != rightList.Count)
                        return Value.FromBoolean(operatorType == TokenType.NotEqual);

                    // Perform element-wise comparison
                    for (int i = 0; i < leftList.Count; i++)
                    {
                        // Use the equality operator for element comparison
                        var elementComparisonResult = PerformComparison(leftList[i], rightList[i], TokenType.EqualEqual);
                        if (!elementComparisonResult.AsBoolean())
                        {
                            // If any element comparison returns false for equality, the lists are not equal
                            return Value.FromBoolean(operatorType == TokenType.NotEqual);
                        }
                    }

                    // If we reach here, all elements are equal
                    return Value.FromBoolean(operatorType == TokenType.EqualEqual);

                default:
                    throw new Exception("Type mismatch or unsupported types for comparison.");
            }
        }

        public Value Visit(BooleanLiteral node) => Value.FromBoolean(node.Value);
        public Value Visit(NumberLiteral node) => Value.FromNumber(node.Value);
        public Value Visit(StringLiteral node) => Value.FromString(node.Value);
        public Value Visit(CharLiteral node) => Value.FromChar(node.Value);

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

        private static Value ApplyCompoundOperation(Value left, Value right, TokenType operatorType)
        {
            switch (left.Type, right.Type)
            {
                case (ValueType.Number or ValueType.Char, ValueType.Number or ValueType.Char):
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

                case (ValueType.String, _):
                    if (operatorType != TokenType.PlusEqual)
                        throw new InvalidOperationException("Compound assignment for strings only supports the += operator.");
                    return Value.FromString(left.AsString() + right.ToString());

                case (ValueType.List, _):
                    if (operatorType != TokenType.PlusEqual)
                        throw new InvalidOperationException("Compound assignment for lists only supports the += operator.");
                    return Value.AddToList(left, right);

                default:
                    throw new InvalidOperationException("Compound assignment is not supported for the given types.");
            }
        }

        public Value Visit(AssignmentExpression node)
        {
            // Evaluate RHS
            Value rhsValue = node.Right.Accept(this);

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
            else if (node.Left is IndexerExpression indexer)
            {
                Value listValue = indexer.Collection.Accept(this);
                Value indexValue = indexer.Index.Accept(this);

                if (listValue.Type != ValueType.List || indexValue.Type != ValueType.Number)
                    throw new InvalidOperationException("Unsupported type or invalid index.");

                List<Value> list = listValue.AsList();
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

        public Value Visit(Variable node) => _scopeManager.LookupVariable(node.Name);

        public void Visit(EmptyStatement node) { }
        public Value Visit(FunctionCall node)
        {
            // If the function is an intrinsic function, invoke it
            if (_intrinsicFunctions.TryGetValue(node.FunctionName, out var intrinsicFunction))
                return intrinsicFunction.Invoke(node, this);

            // Function is not an intrinsic function, look for a user-defined function
            if (!_functions.TryGetValue(node.FunctionName, out var function))
                throw new Exception($"Function {node.FunctionName} is not defined.");

            // Enter a new scope for function execution
            _scopeManager.EnterScope();

            // Prepare for function execution by saving the current context
            int previousLoopDepth = _context.LoopDepth;
            _context.LoopDepth = 0; // Reset loop depth for the new function context
            var previousFlowState = _context.FlowState; // Save the current flow state
            _context.FlowState = ControlFlowState.Normal; // Reset flow state for function execution

            // Set function parameters in the new scope
            for (int i = 0; i < node.Arguments.Count; i++)
            {
                var argValue = node.Arguments[i].Accept(this);
                _scopeManager.SetVariable(function.Parameters[i], argValue);
            }

            // Execute function body
            function.Body.Accept(this);

            // Capture the return value if the function execution resulted in a return
            Value returnValue = 
                _context.FlowState == ControlFlowState.Return 
                ? _context.LastReturnValue : Value.None();

            // Restore the context after function execution
            _context.LoopDepth = previousLoopDepth; // Restore the loop depth to its previous state
            _context.FlowState = previousFlowState; // Restore the flow state to its previous state

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

        public Value Visit(UnaryOperatorExpression node)
        {
            var operandResult = node.Operand.Accept(this);
            TokenType op = node.Operator;
            OperatorFixity fixity = node.Fixity;

            switch (op, fixity)
            {
                case (TokenType.Plus, OperatorFixity.Prefix):
                    return operandResult;
                case (TokenType.Minus, OperatorFixity.Prefix):
                    return Value.FromNumber(-operandResult.AsNumber());
                case (TokenType.Not, OperatorFixity.Prefix):
                    return Value.FromBoolean(!operandResult.AsBoolean());

                case (TokenType.Increment or TokenType.Decrement, _):
                    if (node.Operand is Variable variableNode)
                    {
                        if (operandResult.Type != ValueType.Number && operandResult.Type != ValueType.Char)
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
                        _scopeManager.SetVariable(variableName, Value.FromNumber(newValue));

                        return node.Fixity == OperatorFixity.Prefix ?
                            Value.FromNumber(newValue) : Value.FromNumber(currentValue);
                    }

                    // Check if the operand is an element access in a list
                    if (node.Operand is IndexerExpression indexer)
                    {
                        // Ensure the list and index are valid
                        var listResult = indexer.Collection.Accept(this);
                        var indexResult = indexer.Index.Accept(this);
                        if (listResult.Type != ValueType.List || indexResult.Type != ValueType.Number)
                            throw new InvalidOperationException("Invalid element access in list.");

                        var list = listResult.AsList();
                        var index = (int)indexResult.AsNumber();
                        if (index < 0 || index >= list.Count)
                            throw new IndexOutOfRangeException("List index out of range.");

                        // Ensure the target element is a number or a character
                        if (list[index].Type != ValueType.Number 
                            && list[index].Type != ValueType.Char)
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
                        list[index] = Value.FromNumber(newValue);

                        // Depending on whether it's prefix or postfix, return the new or old value
                        return node.Fixity == OperatorFixity.Prefix ? 
                            Value.FromNumber(newValue) : Value.FromNumber(currentValue);
                    }

                    throw new Exception($"The operand of a {fixity} {op} operator must be a variable or a list element.");


                default:
                    throw new InvalidOperationException($"Unsupported {fixity} operator {op}");
            }
        }

        public void Visit(ExpressionStatement node) => node.Expression.Accept(this);
    }
}