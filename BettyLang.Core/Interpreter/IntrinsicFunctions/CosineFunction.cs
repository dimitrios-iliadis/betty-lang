using BettyLang.Core.AST;

namespace BettyLang.Core.Interpreter.IntrinsicFunctions
{
    public class CosineFunction : IIntrinsicFunction
    {
        public InterpreterValue Invoke(FunctionCall call, IExpressionVisitor visitor)
        {
            if (call.Arguments.Count != 1)
            {
                throw new ArgumentException("cos function requires exactly one numeric argument.");
            }

            var argValue = call.Arguments[0].Accept(visitor);
            if (argValue.Type != ValueType.Number)
            {
                throw new ArgumentException("Argument for cos must be a number.");
            }

            double result = Math.Cos(argValue.AsNumber());
            return InterpreterValue.FromNumber(result);
        }
    }
}