using BettyLang.Core.AST;

namespace BettyLang.Core.Interpreter.IntrinsicFunctions
{
    public class StringConcatenationFunction : IIntrinsicFunction
    {
        public InterpreterValue Invoke(FunctionCall call, IExpressionVisitor visitor)
        {
            // Concatenate all arguments into a single string
            var stringBuilder = new System.Text.StringBuilder();

            foreach (var arg in call.Arguments)
            {
                var argValue = arg.Accept(visitor);

                // Convert each argument to a string regardless of its original type
                string stringValue;
                switch (argValue.Type)
                {
                    case ValueType.Number:
                        stringValue = argValue.AsNumber().ToString();
                        break;
                    case ValueType.Boolean:
                        stringValue = argValue.AsBoolean().ToString();
                        break;
                    case ValueType.String:
                        stringValue = argValue.AsString();
                        break;
                    default:
                        throw new ArgumentException($"Unsupported argument type for string concatenation: {argValue.Type}");
                }

                stringBuilder.Append(stringValue);
            }

            return InterpreterValue.FromString(stringBuilder.ToString());
        }
    }
}