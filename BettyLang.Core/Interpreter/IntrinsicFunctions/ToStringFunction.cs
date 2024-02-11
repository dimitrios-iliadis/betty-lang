using BettyLang.Core.AST;

namespace BettyLang.Core.Interpreter
{
    public static partial class IntrinsicFunctions
    {
        public static InterpreterValue ToStringFunction(FunctionCall call, IExpressionVisitor visitor)
        {
            if (call.Arguments.Count != 1)
            {
                throw new ArgumentException($"{call.FunctionName} function requires exactly one argument.");
            }

            var argResult = call.Arguments[0].Accept(visitor);

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
    }
}