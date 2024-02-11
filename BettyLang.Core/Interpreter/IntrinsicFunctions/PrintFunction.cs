using BettyLang.Core.AST;

namespace BettyLang.Core.Interpreter
{
    public static partial class IntrinsicFunctions
    {
        public static InterpreterValue PrintFunction(FunctionCall call, IExpressionVisitor visitor)
        {
            // Iterate over all arguments, converting each to a string and printing it
            foreach (var arg in call.Arguments)
            {
                var argResult = arg.Accept(visitor);
                string printValue;

                // Perform implicit conversion based on the argument's type
                switch (argResult.Type)
                {
                    case ValueType.Number:
                        printValue = argResult.AsNumber().ToString();
                        break;
                    case ValueType.Boolean:
                        printValue = argResult.AsBoolean().ToString();
                        break;
                    case ValueType.String:
                        printValue = argResult.AsString();
                        break;
                    case ValueType.None:
                        printValue = "None";
                        break;
                    default:
                        throw new ArgumentException($"Unsupported argument type for printing: {argResult.Type}");
                }

                // Print the converted string value
                Console.Write(printValue);
            }
            if (call.FunctionName == "println")
                Console.Write("\n"); // Ensure that there's a newline after all arguments are printed

            return InterpreterValue.None();
        }
    }
}