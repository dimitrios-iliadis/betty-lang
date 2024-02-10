using BettyLang.Core.AST;

namespace BettyLang.Core.Interpreter.IntrinsicFunctions
{
    public class SquareRootFunction : IIntrinsicFunction
    {
        public InterpreterValue Invoke(FunctionCall call, IExpressionVisitor visitor)
        {
            if (call.Arguments.Count != 1)
            {
                throw new ArgumentException("sqrt function requires exactly one numeric argument.");
            }

            var argValue = call.Arguments[0].Accept(visitor);
            if (argValue.Type != ValueType.Number)
            {
                throw new ArgumentException("Argument for sqrt must be a number.");
            }

            double result = Math.Sqrt(argValue.AsNumber());
            return InterpreterValue.FromNumber(result);
        }
    }
}