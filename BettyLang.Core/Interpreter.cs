using BettyLang.Core.AST;

namespace BettyLang.Core
{
    public class BreakException : Exception { }
    public class ContinueException : Exception { }
    public class ReturnException : Exception
    {
        public object ReturnValue { get; }

        public ReturnException(object returnValue)
        {
            ReturnValue = returnValue;
        }
    }

    public class Interpreter : INodeVisitor
    {
        private readonly Parser _parser;

        private readonly Dictionary<string, FunctionDefinitionNode> _functions = new();
        private readonly Stack<Dictionary<string, object>> _scopes = new();

        private static readonly Dictionary<string, Func<double, double>> _mathFunctions = new()
        {
            {"sin", Math.Sin},
            {"cos", Math.Cos},
            {"tan", Math.Tan},
            {"sqrt", Math.Sqrt}
        };

        private static readonly HashSet<string> _builtInFunctionNames = new()
        {
            "print",
            "println",
            "input",

            "sin",
            "cos",
            "tan",
            "sqrt"
        };

        private bool _isInLoop = false;

        public Interpreter(Parser parser)
        {
            _parser = parser;
        }

        public object Visit(ProgramNode node)
        {
            // Visit each function definition and store it in a dictionary
            foreach (var function in node.Functions)
                function.Accept(this);

            if (_functions.TryGetValue("main", out var mainFunction))
            {
                if (mainFunction.Parameters.Count != 0)
                    throw new Exception("main() function cannot have parameters.");

                // Execute the main function
                return Visit(new FunctionCallNode("main", []));
            }
            else
            {
                throw new Exception("No main function found.");
            }
        }

        public object Visit(TernaryOperatorNode node)
        {
            var conditionResult = (bool)node.Condition.Accept(this);
            return conditionResult ? node.TrueExpression.Accept(this) : node.FalseExpression.Accept(this);
        }

        public object Visit(ReturnStatementNode node)
        {
            object returnValue = null;
            if (node.ReturnValue != null)
            {
                returnValue = node.ReturnValue.Accept(this);
            }

            // Use the exception to carry the return value
            throw new ReturnException(returnValue);
        }

        private object PerformComparison(object leftValue, object rightValue, TokenType operatorType)
        {
            if (leftValue is double leftDouble && rightValue is double rightDouble)
            {
                return operatorType switch
                {
                    TokenType.Equal => leftDouble == rightDouble,
                    TokenType.NotEqual => leftDouble != rightDouble,
                    TokenType.LessThan => leftDouble < rightDouble,
                    TokenType.LessThanOrEqual => leftDouble <= rightDouble,
                    TokenType.GreaterThan => leftDouble > rightDouble,
                    TokenType.GreaterThanOrEqual => leftDouble >= rightDouble,
                    _ => throw new Exception($"Unsupported operator for number comparison: {operatorType}")
                };
            }
            else if (leftValue is string leftString && rightValue is string rightString)
            {
                return operatorType switch
                {
                    TokenType.Equal => leftString == rightString,
                    TokenType.NotEqual => leftString != rightString,
                    _ => throw new Exception($"Unsupported operator for string comparison: {operatorType}")
                };
            }
            else if (leftValue is bool leftBool && rightValue is bool rightBool)
            {
                return operatorType switch
                {
                    TokenType.Equal => leftBool == rightBool,
                    TokenType.NotEqual => leftBool != rightBool,
                    _ => throw new Exception($"Unsupported operator for boolean comparison: {operatorType}")
                };
            }
            else
                throw new Exception("Type mismatch or unsupported types for comparison.");
        }

        public object Visit(BreakStatementNode node)
        {
            if (!_isInLoop)
            {
                throw new Exception("Break statement not inside a loop");
            }

            throw new BreakException();
        }

        public object Visit(ContinueStatementNode node)
        {
            if (!_isInLoop)
            {
                throw new Exception("Continue statement not inside a loop");
            }

            throw new ContinueException();
        }

        public object Visit(WhileStatementNode node)
        {
            _isInLoop = true;

            try
            {
                while ((bool)node.Condition.Accept(this))
                {
                    try
                    {
                        node.Body.Accept(this);
                    }
                    catch (ContinueException)
                    {
                        continue;
                    }
                }
            }
            catch (BreakException) { }
            finally { _isInLoop = false; }

            return null;
        }

        public object Visit(IfStatementNode node)
        {
            var conditionResult = (bool)node.Condition.Accept(this);

            if (conditionResult)
            {
                return node.ThenStatement.Accept(this);
            }
            else
            {
                foreach (var (Condition, Statement) in node.ElseIfStatements)
                {
                    if ((bool)Condition.Accept(this))
                    {
                        return Statement.Accept(this);
                    }
                }

                if (node.ElseStatement != null)
                {
                    return node.ElseStatement.Accept(this);
                }
            }

            return null;
        }

        public object Visit(BinaryOperatorNode node)
        {
            var leftResult = node.Left.Accept(this);
            var rightResult = node.Right.Accept(this);

            if (leftResult is bool leftBool && rightResult is bool rightBool)
            {
                switch (node.Operator.Type)
                {
                    case TokenType.And:
                        return leftBool && rightBool;
                    case TokenType.Or:
                        return leftBool || rightBool;
                }
            }

            if (node.Operator.Type == TokenType.Plus &&
                (leftResult is string || rightResult is string))
            {
                string leftString = leftResult.ToString()!;
                string rightString = rightResult.ToString()!;
                return leftString + rightString;
            }

            if (leftResult is double leftOperand && rightResult is double rightOperand)
            {
                switch (node.Operator.Type)
                {
                    case TokenType.Plus:
                        return leftOperand + rightOperand;
                    case TokenType.Minus:
                        return leftOperand - rightOperand;
                    case TokenType.Star:
                        return leftOperand * rightOperand;
                    case TokenType.Slash:
                        return leftOperand / rightOperand;
                    case TokenType.Caret:
                        return Math.Pow(leftOperand, rightOperand);
                    case TokenType.Percent:
                        return leftOperand % rightOperand;
                }
            }

            switch (node.Operator.Type)
            {
                case TokenType.Equal:
                case TokenType.NotEqual:
                case TokenType.LessThan:
                case TokenType.LessThanOrEqual:
                case TokenType.GreaterThan:
                case TokenType.GreaterThanOrEqual:
                    return PerformComparison(leftResult, rightResult, node.Operator.Type);
            }

            throw new Exception($"Unsupported binary operator: {node.Operator.Type}");
        }

        public object Visit(BooleanLiteralNode node) => node.Value;

        public double Visit(NumberLiteralNode node) => node.Value;

        public string Visit(StringLiteralNode node) => node.Value;

        public object Visit(CompoundStatementNode node)
        {
            foreach (var statement in node.Statements)
                statement.Accept(this);

            return null;
        }

        public object Visit(AssignmentNode node)
        {
            if (node.Left is VariableNode variableNode)
            {
                string variableName = variableNode.Value;
                var rightResult = node.Right.Accept(this);
                AssignVariable(variableName, rightResult);
                return null;
            }
            else
                throw new Exception("The left-hand side of an assignment must be a variable.");
        }

        public object Visit(VariableNode node)
        {
            string variableName = node.Value;
            return LookupVariable(variableName);
        }

        public object Visit(EmptyStatementNode node) => null;

        private object HandleBuiltInFunction(FunctionCallNode node)
        {
            if (node.FunctionName == "print" || node.FunctionName == "println")
                return HandlePrintFunction(node);
            else if (node.FunctionName == "input")
                return HandleInputFunction(node);
            else if (_mathFunctions.TryGetValue(node.FunctionName, out var func))
                return HandleMathFunction(node, func);
            else
                throw new Exception($"Unknown built-in function {node.FunctionName}");
        }

        private object HandleInputFunction(FunctionCallNode node)
        {
            // Check for correct number of arguments
            if (node.Arguments.Count > 1)
                throw new Exception("Input function requires at most one argument, which can be a prompt string.");

            // If an argument is provided, display it as a prompt
            if (node.Arguments.Count == 1)
                Console.Write(node.Arguments[0].Accept(this));

            // Read input from the user
            string userInput = Console.ReadLine() ?? string.Empty;

            // Try to parse the input as a double. If it fails, treat it as a string
            object value = double.TryParse(userInput, out double numericValue) ? numericValue : userInput;

            return value;
        }

        private object HandlePrintFunction(FunctionCallNode node)
        {
            // Ensure that only one argument is provided
            if (node.Arguments.Count != 1)
                throw new Exception($"{node.FunctionName} function requires exactly one argument.");

            // Evaluate the argument
            var argResult = node.Arguments[0].Accept(this);

            if (node.FunctionName == "println")
                Console.WriteLine(argResult);
            else
                Console.Write(argResult);

            return null;
        }


        private object HandleMathFunction(FunctionCallNode node, Func<double, double> func)
        {
            if (node.Arguments.Count != 1)
                throw new Exception($"{node.FunctionName}() takes exactly one argument");

            if (node.Arguments[0].Accept(this) is not double)
                throw new Exception($"{node.FunctionName}() must be called with a numeric argument");

            var argValue = (double)node.Arguments[0].Accept(this);
            return func(argValue);
        }

        private bool IsBuiltInFunction(string functionName) => _builtInFunctionNames.Contains(functionName);

        public object Visit(FunctionCallNode node)
        {
            if (IsBuiltInFunction(node.FunctionName))
                return HandleBuiltInFunction(node);

            if (!_functions.TryGetValue(node.FunctionName, out var function))
                throw new Exception($"Function {node.FunctionName} is not defined.");

            // Enter a new scope for function execution
            EnterScope();

            object returnValue = null;

            // Store the current loop context
            bool wasInLoop = _isInLoop;
            _isInLoop = false; // Reset isInLoop because we're entering a new function context

            try
            {
                // Set function parameters in the new scope
                for (int i = 0; i < node.Arguments.Count; i++)
                {
                    var argValue = node.Arguments[i].Accept(this);
                    _scopes.Peek()[function.Parameters[i]] = argValue;
                }

                // Execute function body
                function.Body.Accept(this);
            }
            catch (ReturnException ex)
            {
                // Catch the return value from the function
                returnValue = ex.ReturnValue;
            }
            finally
            {
                // Restore the loop context after function execution
                _isInLoop = wasInLoop;

                // Exit the function scope regardless of how we leave the function
                ExitScope();
            }

            return returnValue ?? null; // return null or default if no value was returned
        }

        public object Visit(FunctionDefinitionNode node)
        {
            if (IsBuiltInFunction(node.FunctionName))
                throw new Exception($"Function name '{node.FunctionName}' is reserved for built-in functions.");

            _functions[node.FunctionName] = node;
            return null;
        }

        public object Visit(UnaryOperatorNode node)
        {
            TokenType op = node.Operator.Type;
            var operandResult = node.Expression.Accept(this);

            switch (op)
            {
                case TokenType.Plus:
                    return +(double)operandResult;
                case TokenType.Minus:
                    return -(double)operandResult;
                case TokenType.Not:
                    return !(bool)operandResult;
            }

            throw new InvalidOperationException($"Unsupported unary operator {op}");
        }

        public object Interpret()
        {
            var tree = _parser.Parse();
            return tree.Accept(this);
        }

        private void EnterScope()
        {
            _scopes.Push(new Dictionary<string, object>());
        }

        private void ExitScope()
        {
            _scopes.Pop();
        }

        private void AssignVariable(string name, object value)
        {
            var currentScope = _scopes.Peek();
            currentScope[name] = value;
        }

        private object LookupVariable(string name)
        {
            foreach (var scope in _scopes)
            {
                if (scope.TryGetValue(name, out var value))
                {
                    return value;
                }
            }
            throw new Exception($"Variable '{name}' not found in any scope");
        }
    }
}