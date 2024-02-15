using BettyLang.Core.AST;

namespace BettyLang.Core.Interpreter
{
    public static partial class IntrinsicFunctions
    {
        public static InterpreterResult PrintFunction(FunctionCall call, IExpressionVisitor visitor)
        {
            // Concatenate all arguments into a single string
            var stringBuilder = new System.Text.StringBuilder();

            foreach (var arg in call.Arguments)
            {
                var argValue = arg.Accept(visitor);
                stringBuilder.Append(argValue.ToString());
            }

            if (call.FunctionName == "println")
                stringBuilder.Append('\n');

            Console.Write(stringBuilder.ToString());

            return InterpreterResult.None();
        }
    }
}