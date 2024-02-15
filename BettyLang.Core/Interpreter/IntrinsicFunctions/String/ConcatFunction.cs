using BettyLang.Core.AST;

namespace BettyLang.Core.Interpreter
{
    public static partial class IntrinsicFunctions
    {
        public static InterpreterResult ConcatFunction(FunctionCall call, IExpressionVisitor visitor)
        {
            // Concatenate all arguments into a single string
            var stringBuilder = new System.Text.StringBuilder();

            foreach (var arg in call.Arguments)
            {
                var argValue = arg.Accept(visitor);
                stringBuilder.Append(argValue.ToString());
            }

            return InterpreterResult.FromString(stringBuilder.ToString());
        }
    }
}