using BettyLang.Core.AST;

namespace BettyLang.Core.Interpreter
{
    public static partial class IntrinsicFunctions
    {
        public static InterpreterResult ToNumberFunction(FunctionCall call, IExpressionVisitor visitor)
        {
            if (call.Arguments.Count != 1)
            {
                throw new ArgumentException($"{call.FunctionName} function requires exactly one argument.");
            }

            var argResult = call.Arguments[0].Accept(visitor);

            double numberValue;
            switch (argResult.Type)
            {
                case ResultType.Number:
                    return argResult;

                case ResultType.Char:
                    numberValue = argResult.AsNumber(); // Get the numeric value of the character.
                    break;

                case ResultType.Boolean:
                    numberValue = argResult.AsBoolean() ? 1 : 0;
                    break;

                case ResultType.String:
                    if (!double.TryParse(argResult.AsString(), out numberValue))
                    {
                        throw new ArgumentException($"Could not convert string '{argResult.AsString()}' to number.");
                    }
                    break;

                default:
                    throw new InvalidOperationException($"Conversion to number not supported for type {argResult.Type}.");
            }

            return InterpreterResult.FromNumber(numberValue);
        }
    }
}