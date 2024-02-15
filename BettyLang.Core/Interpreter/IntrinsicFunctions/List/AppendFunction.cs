using BettyLang.Core.AST;

namespace BettyLang.Core.Interpreter
{ 
    public partial class IntrinsicFunctions
    {
        public static InterpreterResult AppendFunction(FunctionCall call, IExpressionVisitor visitor)
        {
            if (call.Arguments.Count != 2)
            {
                throw new ArgumentException("append function requires exactly two arguments: a list and an element to append.");
            }

            var listResult = call.Arguments[0].Accept(visitor);
            if (listResult.Type != ResultType.List)
            {
                throw new InvalidOperationException("The first argument of append must be a list.");
            }

            var element = call.Arguments[1].Accept(visitor);
            var list = listResult.AsList();
            list.Add(element);

            // Return the modified list as a new InterpreterResult.
            return InterpreterResult.FromList(new List<InterpreterResult>(list));
        }
    }
}