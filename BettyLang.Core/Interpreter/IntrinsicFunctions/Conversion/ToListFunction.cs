using BettyLang.Core.AST;

namespace BettyLang.Core.Interpreter
{
    public static partial class IntrinsicFunctions
    {
        public static InterpreterResult ToListFunction(FunctionCall call, IExpressionVisitor visitor)
        {
            if (call.Arguments.Count != 1)
            {
                throw new ArgumentException($"{call.FunctionName} function requires exactly one argument.");
            }

            var argResult = call.Arguments[0].Accept(visitor);

            if (argResult.Type != ResultType.String)
            {
                throw new InvalidOperationException($"Conversion to list not supported for type {argResult.Type}.");
            }

            var str = argResult.AsString();
            var list = new List<InterpreterResult>();
            foreach (var c in str)
                list.Add(InterpreterResult.FromChar(c));

            return InterpreterResult.FromList(list);
        }
    }
}