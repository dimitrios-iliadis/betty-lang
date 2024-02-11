using BettyLang.Core.AST;

namespace BettyLang.Core.Interpreter
{
    public static partial class IntrinsicFunctions
    {
        public static InterpreterValue StringConcatFunction(FunctionCall call, IExpressionVisitor visitor)
        {
            // Concatenate all arguments into a single string
            var stringBuilder = new System.Text.StringBuilder();

            foreach (var arg in call.Arguments)
            {
                var argValue = arg.Accept(visitor);

                // Convert each argument to a string regardless of its original type
                string stringValue = argValue.Type switch
                {
                    ValueType.Number => argValue.AsNumber().ToString(),
                    ValueType.Boolean => argValue.AsBoolean().ToString(),
                    ValueType.String => argValue.AsString(),
                    ValueType.None => "None",
                    _ => throw new ArgumentException($"Unsupported argument type for string concatenation: {argValue.Type}"),
                };
                stringBuilder.Append(stringValue);
            }

            return InterpreterValue.FromString(stringBuilder.ToString());
        }
    }
}