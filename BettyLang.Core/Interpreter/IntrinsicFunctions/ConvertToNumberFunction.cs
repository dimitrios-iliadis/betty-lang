using BettyLang.Core.AST;

namespace BettyLang.Core.Interpreter.IntrinsicFunctions
{
    public class ConvertToNumberFunction : IIntrinsicFunction
    {
        public InterpreterValue Invoke(FunctionCall call, IExpressionVisitor visitor)
        {
            if (call.Arguments.Count != 1)
            {
                throw new ArgumentException("to_num function requires exactly one argument.");
            }

            var argResult = call.Arguments[0].Accept(visitor);

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
    }
}