using BettyLang.Core.AST;

namespace BettyLang.Core.Interpreter
{
    public class Interpreter : IStatementVisitor, IExpressionVisitor
    {
        private readonly Parser _parser;

        private readonly Dictionary<string, FunctionDefinition> _functions;
        private readonly ScopeManager _scopeManager;
        private delegate InterpreterValue BuiltInFunctionHandler(FunctionCall node);
        private readonly Dictionary<string, BuiltInFunctionHandler> _builtInFunctions;

        private readonly InterpreterContext _context;

        private static readonly Dictionary<string, Func<double, double>> _mathFunctions = new()
        {
            {"sin", Math.Sin},
            {"cos", Math.Cos},
            {"tan", Math.Tan},
            {"sqrt", Math.Sqrt}
        };

        public Interpreter(Parser parser)
        {
            _parser = parser;

            _functions = new Dictionary<string, FunctionDefinition>();
            _scopeManager = new ScopeManager();
            _context = new InterpreterContext();

            _builtInFunctions = new Dictionary<string, BuiltInFunctionHandler>
            {
                { "print", PrintFunction },
                { "println", PrintFunction },
                { "input", InputFunction },

                { "tostr", ToStringFunction },
                { "tonum", ToNumberFunction },
                { "tobool", ToBooleanFunction },

                {"strlen", StringLengthFunction }
            };

            // Dynamically add math functions to the built-in functions dictionary
            foreach (var mathFunc in _mathFunctions.Keys)
            {
                _builtInFunctions.Add(mathFunc, MathFunctionHandler);
            }
        }

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
                    _context.ResetFlowState(); // Prepare for the next iteration
                    continue;
                }

                // Break out of the loop on break
                if (_context.FlowState == ControlFlowState.Break)
                {
                    break;
                }
            }
            _context.IsInLoop = false;
            if (_context.FlowState == ControlFlowState.Break)
            {
                _context.ResetFlowState(); // Loop exited because of a break, reset flow for the code outside the loop
            }
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
                    if (leftResult.Type == ValueType.Boolean && rightResult.Type == ValueType.Boolean)
                    {
                        bool leftBoolean = leftResult.AsBoolean();
                        bool rightBoolean = rightResult.AsBoolean();
                        return InterpreterValue.FromBoolean(
                            node.Operator.Type == TokenType.And ? leftBoolean && rightBoolean : leftBoolean || rightBoolean);
                    }
                    break;

                case TokenType.Plus:
                    // Concatenate if both operands are strings
                    if (leftResult.Type == ValueType.String && rightResult.Type == ValueType.String)
                    {
                        string leftString = leftResult.AsString();
                        string rightString = rightResult.AsString();
                        return InterpreterValue.FromString(leftString + rightString);
                    }
                    else if (leftResult.Type == ValueType.Number && rightResult.Type == ValueType.Number)
                    {
                        // Handle numeric addition if both operands are numbers
                        double leftNumber = leftResult.AsNumber();
                        double rightNumber = rightResult.AsNumber();
                        return InterpreterValue.FromNumber(leftNumber + rightNumber);
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid operation for mixed or incompatible types.");
                    }

                case TokenType.Minus:
                case TokenType.Star:
                case TokenType.Slash:
                case TokenType.Caret:
                case TokenType.Modulo:
                    if (leftResult.Type == ValueType.Number && rightResult.Type == ValueType.Number)
                    {
                        double leftOperand = leftResult.AsNumber();
                        double rightOperand = rightResult.AsNumber();
                        double result = node.Operator.Type switch
                        {
                            TokenType.Minus => leftOperand - rightOperand,
                            TokenType.Star => leftOperand * rightOperand,
                            TokenType.Slash => leftOperand / rightOperand,
                            TokenType.Caret => Math.Pow(leftOperand, rightOperand),
                            TokenType.Modulo => leftOperand % rightOperand,
                            _ => throw new Exception($"Unsupported binary operator for numbers: {node.Operator.Type}")
                        };
                        return InterpreterValue.FromNumber(result);
                    }
                    else
                    {
                        // Handle error
                        throw new InvalidOperationException("Arithmetic operations require both operands to be numbers.");
                    }

                case TokenType.EqualEqual:
                case TokenType.NotEqual:
                case TokenType.LessThan:
                case TokenType.LessThanOrEqual:
                case TokenType.GreaterThan:
                case TokenType.GreaterThanOrEqual:
                    return PerformComparison(leftResult, rightResult, node.Operator.Type);
            }

            throw new Exception($"Unsupported binary operator: {node.Operator.Type} for operand types {leftResult.Type} and {rightResult.Type}");
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

        public InterpreterValue Visit(Variable node)
        {
            return _scopeManager.LookupVariable(node.Name);
        }

        public void Visit(EmptyStatement node) { }

        private InterpreterValue MathFunctionHandler(FunctionCall node)
        {
            var mathFunc = _mathFunctions.GetValueOrDefault(node.FunctionName)!;

            // Validate there is exactly one argument
            if (node.Arguments.Count != 1)
            {
                throw new ArgumentException($"{node.FunctionName} requires exactly one numeric argument.");
            }

            // Evaluate the argument
            var argResult = node.Arguments[0].Accept(this);
            if (argResult.Type != ValueType.Number)
            {
                throw new ArgumentException($"Argument for {node.FunctionName} must be a number.");
            }

            // Execute the math function
            var result = mathFunc(argResult.AsNumber());

            // Return the result
            return InterpreterValue.FromNumber(result);
        }

        private InterpreterValue StringLengthFunction(FunctionCall node)
        {
            // Ensure exactly one argument is provided
            if (node.Arguments.Count != 1)
            {
                throw new ArgumentException("strlen function requires exactly one argument.");
            }

            // Evaluate the argument
            var argResult = node.Arguments[0].Accept(this);

            // Ensure the argument is a string
            if (argResult.Type != ValueType.String)
            {
                throw new ArgumentException("Argument for strlen function must be a string.");
            }

            // Return the length of the string
            return InterpreterValue.FromNumber(argResult.AsString().Length);
        }

        private InterpreterValue ToStringFunction(FunctionCall node)
        {
            // Ensure exactly one argument is provided
            if (node.Arguments.Count != 1)
            {
                throw new ArgumentException("String function requires exactly one argument.");
            }

            // Evaluate the argument
            var argResult = node.Arguments[0].Accept(this);

            // Convert the argument result to string based on its type
            return InterpreterValue.FromString(argResult.Type switch
            {
                ValueType.Number => argResult.AsNumber().ToString(),
                ValueType.Boolean => argResult.AsBoolean().ToString(),
                ValueType.String => argResult.AsString(),
                ValueType.None => "None",
                _ => throw new InvalidOperationException("Unsupported type for string conversion.")
            });
        }

        private InterpreterValue ToBooleanFunction(FunctionCall node)
        {
            if (node.Arguments.Count != 1)
            {
                throw new ArgumentException("Boolean function requires exactly one argument.");
            }

            var argResult = node.Arguments[0].Accept(this);

            bool booleanValue;
            switch (argResult.Type)
            {
                case ValueType.Number:
                    // Any number other than 0 is true, 0 is false
                    booleanValue = argResult.AsNumber() != 0;
                    break;

                case ValueType.String:
                    // Consider non-empty strings as true, and optionally parse "true" and "false"
                    var str = argResult.AsString();
                    if (bool.TryParse(str, out bool parsedValue))
                    {
                        // Successfully parsed "true" or "false"
                        booleanValue = parsedValue;
                    }
                    else
                    {
                        // Any non-empty string is considered true, empty string false
                        booleanValue = !string.IsNullOrEmpty(str);
                    }
                    break;

                case ValueType.Boolean:
                    // Return the boolean value directly
                    return argResult;

                default:
                    throw new InvalidOperationException($"Conversion to boolean not supported for type {argResult.Type}.");
            }

            return InterpreterValue.FromBoolean(booleanValue);
        }

        private InterpreterValue ToNumberFunction(FunctionCall node)
        {
            if (node.Arguments.Count != 1)
            {
                throw new ArgumentException("Number function requires exactly one argument.");
            }

            var argResult = node.Arguments[0].Accept(this);

            double numberValue;
            switch (argResult.Type)
            {
                case ValueType.Number:
                    return argResult;

                case ValueType.Boolean:
                    numberValue = argResult.AsBoolean() ? 1 : 0;
                    break;

                case ValueType.String:
                    if (!double.TryParse(argResult.AsString(), out numberValue))
                    {
                        throw new ArgumentException($"Could not convert string '{argResult.AsString()}' to number.");
                    }
                    break;

                default:
                    throw new InvalidOperationException($"Conversion to number not supported for type {argResult.Type}.");
            }

            return InterpreterValue.FromNumber(numberValue);
        }

        private InterpreterValue InputFunction(FunctionCall node)
        {
            // Check for correct number of arguments
            if (node.Arguments.Count > 1)
                throw new Exception("Input function requires at most one argument, which can be a prompt string.");

            // If an argument is provided, display it as a prompt
            if (node.Arguments.Count == 1)
                Console.Write(node.Arguments[0].Accept(this).AsString());

            // Read input from the user
            string userInput = Console.ReadLine() ?? string.Empty;

            return InterpreterValue.FromString(userInput);
        }

        private InterpreterValue PrintFunction(FunctionCall node)
        {
            // Ensure that only one argument is provided
            if (node.Arguments.Count != 1)
                throw new Exception($"{node.FunctionName} function requires exactly one argument.");

            // Evaluate the argument
            var argResult = node.Arguments[0].Accept(this).AsString();

            if (node.FunctionName == "println")
                Console.WriteLine(argResult);
            else
                Console.Write(argResult);

            return InterpreterValue.None();
        }

        public void Visit(FunctionCallStatement node) => Visit(node.FunctionCall);

        public InterpreterValue Visit(FunctionCall node)
        {
            if (_builtInFunctions.TryGetValue(node.FunctionName, out var handler))
            {
                // Handle built-in functions directly
                return handler(node);
            }

            if (!_functions.TryGetValue(node.FunctionName, out var function))
            {
                throw new Exception($"Function {node.FunctionName} is not defined.");
            }

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
            if (_builtInFunctions.ContainsKey(node.FunctionName))
                throw new Exception($"Function name '{node.FunctionName}' is reserved for built-in functions.");

            _functions[node.FunctionName] = node;
        }

        public InterpreterValue Visit(UnaryOperatorExpression node)
        {
            TokenType @operator = node.Operator.Type;
            var operandResult = node.Expression.Accept(this);

            return @operator switch
            {
                TokenType.Plus => operandResult,
                TokenType.Minus => InterpreterValue.FromNumber(-operandResult.AsNumber()),
                TokenType.Not => InterpreterValue.FromBoolean(!operandResult.AsBoolean()),
                _ => throw new InvalidOperationException($"Unsupported unary operator {@operator}")
            };
        }
    }
}