using BettyLang.Core.AST;

namespace BettyLang.Core.Interpreter.IntrinsicFunctions
{
    public class InputFunction : IIntrinsicFunction
    {
        public InterpreterValue Invoke(FunctionCall call, IExpressionVisitor visitor)
        {
            if (call.Arguments.Count > 1)
            {
                throw new Exception("Input function requires at most one argument, which can be a prompt string.");
            }

            // If an argument is provided, use the visitor to evaluate it and display as a prompt
            if (call.Arguments.Count == 1)
            {
                var promptValue = call.Arguments[0].Accept(visitor);
                Console.Write(promptValue.AsString());
            }

            // Read input from the user
            string userInput = Console.ReadLine() ?? string.Empty;

            return InterpreterValue.FromString(userInput);
        }
    }
}