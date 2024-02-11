using BettyLang.Core.AST;


namespace BettyLang.Core.Interpreter
{
    public partial class Interpreter(Parser parser) : IStatementVisitor, IExpressionVisitor
    {
        private readonly Parser _parser = parser;
        private readonly Dictionary<string, FunctionDefinition> _functions = [];
        private readonly ScopeManager _scopeManager = new();
        private readonly InterpreterContext _context = new();

        public InterpreterValue Interpret()
        {
            var tree = _parser.Parse();
            return tree.Accept(this);
        }

        public InterpreterValue Visit(Program node)
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

        public InterpreterValue Visit(TernaryOperatorExpression node)
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
                _context.LastReturnValue = InterpreterValue.None();
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

        public void Visit(WhileStatement node)
        {
            _context.IsInLoop = true;

            while (node.Condition.Accept(this).AsBoolean() && _context.FlowState == ControlFlowState.Normal)
            {
                node.Body.Accept(this);

                // Handle continue
                if (_context.FlowState == ControlFlowState.Continue)
                {
                    _context.FlowState = ControlFlowState.Normal; // Prepare for the next iteration
                    continue;
                }

                // Break out of the loop on break
                if (_context.FlowState == ControlFlowState.Break)
                    break;
            }

            _context.IsInLoop = false;

            if (_context.FlowState == ControlFlowState.Break)
                _context.FlowState = ControlFlowState.Normal; // Loop exited because of a break, reset flow for the code outside the loop
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

        public InterpreterValue Visit(BinaryOperatorExpression node)
        {
            var leftResult = node.Left.Accept(this);
            var rightResult = node.Right.Accept(this);

            switch (node.Operator.Type)
            {
                case TokenType.And:
                case TokenType.Or:
                    if (leftResult.Type != ValueType.Boolean || rightResult.Type != ValueType.Boolean)
                    {
                        throw new InvalidOperationException("Logical operations require both operands to be boolean.");
                    }
                    bool leftBoolean = leftResult.AsBoolean();
                    bool rightBoolean = rightResult.AsBoolean();
                    return InterpreterValue.FromBoolean(
                        node.Operator.Type == TokenType.And ? leftBoolean && rightBoolean : leftBoolean || rightBoolean);

                case TokenType.Plus:
                    if (leftResult.Type == ValueType.String || rightResult.Type == ValueType.String)
                    {
                        return InterpreterValue.FromString(leftResult.ToString() + rightResult.ToString());
                    }
                    goto case TokenType.Minus;
                case TokenType.Minus:
                case TokenType.Star:
                case TokenType.Slash:
                case TokenType.Caret:
                case TokenType.Modulo:
                    if (leftResult.Type != ValueType.Number || rightResult.Type != ValueType.Number)
                    {
                        throw new InvalidOperationException("Arithmetic operations require both operands to be numbers.");
                    }
                    return PerformArithmeticOperation(leftResult, rightResult, node.Operator.Type);

                case TokenType.EqualEqual:
                case TokenType.NotEqual:
                case TokenType.LessThan:
                case TokenType.LessThanOrEqual:
                case TokenType.GreaterThan:
                case TokenType.GreaterThanOrEqual:
                    return PerformComparison(leftResult, rightResult, node.Operator.Type);

                default:
                    throw new Exception($"Unsupported binary operator: {node.Operator.Type} for operand types {leftResult.Type} and {rightResult.Type}");
            }
        }

        private static InterpreterValue PerformArithmeticOperation(InterpreterValue leftResult, InterpreterValue rightResult, TokenType operatorType)
        {
            double leftNumber = leftResult.AsNumber();
            double rightNumber = rightResult.AsNumber();

            return operatorType switch
            {
                TokenType.Plus => InterpreterValue.FromNumber(leftNumber + rightNumber),
                TokenType.Minus => InterpreterValue.FromNumber(leftNumber - rightNumber),
                TokenType.Star => InterpreterValue.FromNumber(leftNumber * rightNumber),
                TokenType.Slash => InterpreterValue.FromNumber(leftNumber / rightNumber),
                TokenType.Caret => InterpreterValue.FromNumber(Math.Pow(leftNumber, rightNumber)),
                TokenType.Modulo => InterpreterValue.FromNumber(leftNumber % rightNumber),
                _ => throw new Exception($"Unsupported arithmetic operator: {operatorType}")
            };
        }

        private static InterpreterValue PerformComparison(InterpreterValue leftResult, InterpreterValue rightResult, TokenType operatorType)
        {
            switch (leftResult.Type, rightResult.Type)
            {
                case (ValueType.Number, ValueType.Number):
                    double leftNumber = leftResult.AsNumber();
                    double rightNumber = rightResult.AsNumber();
                    return InterpreterValue.FromBoolean(operatorType switch
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
                    return InterpreterValue.FromBoolean(operatorType switch
                    {
                        TokenType.EqualEqual => leftString == rightString,
                        TokenType.NotEqual => leftString != rightString,
                        _ => throw new Exception($"Unsupported operator for string comparison: {operatorType}")
                    });

                case (ValueType.Boolean, ValueType.Boolean):
                    bool leftBoolean = leftResult.AsBoolean();
                    bool rightBoolean = rightResult.AsBoolean();
                    return InterpreterValue.FromBoolean(operatorType switch
                    {
                        TokenType.EqualEqual => leftBoolean == rightBoolean,
                        TokenType.NotEqual => leftBoolean != rightBoolean,
                        _ => throw new Exception($"Unsupported operator for boolean comparison: {operatorType}")
                    });

                default:
                    throw new Exception("Type mismatch or unsupported types for comparison.");
            }
        }

        public InterpreterValue Visit(BooleanLiteral node) => InterpreterValue.FromBoolean(node.Value);
        public InterpreterValue Visit(NumberLiteral node) => InterpreterValue.FromNumber(node.Value);
        public InterpreterValue Visit(StringLiteral node) => InterpreterValue.FromString(node.Value);

        public void Visit(CompoundStatement node)
        {
            foreach (var statement in node.Statements)
            {
                statement.Accept(this);

                // Handle control flow changes
                if (_context.FlowState == ControlFlowState.Return)
                {
                    // Return statement encountered, exit the compound statement
                    return;
                }
            }
        }

        public void Visit(AssignmentStatement node)
        {
            if (node.Left is Variable variableNode)
            {
                var rightResult = node.Right.Accept(this);
                _scopeManager.SetVariable(variableNode.Name, rightResult);
            }
            else
                throw new Exception("The left-hand side of an assignment must be a variable.");
        }

        public InterpreterValue Visit(Variable node) => _scopeManager.LookupVariable(node.Name);

        public void Visit(EmptyStatement node) { }

        public void Visit(FunctionCallStatement node) => Visit(node.FunctionCall);

        public InterpreterValue Visit(FunctionCall node)
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
            InterpreterValue returnValue = _context.FlowState == ControlFlowState.Return ? _context.LastReturnValue : InterpreterValue.None();

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

        public InterpreterValue Visit(UnaryOperatorExpression node)
        {
            TokenType op = node.Operator.Type;
            var operandResult = node.Expression.Accept(this);

            return op switch
            {
                TokenType.Plus => operandResult,
                TokenType.Minus => InterpreterValue.FromNumber(-operandResult.AsNumber()),
                TokenType.Not => InterpreterValue.FromBoolean(!operandResult.AsBoolean()),
                _ => throw new InvalidOperationException($"Unsupported unary operator {op}")
            };
        }
    }
}