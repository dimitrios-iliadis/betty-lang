using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class FunctionCall : Expression
    {
        public string FunctionName { get; }
        public List<Expression> Arguments { get; }

        public FunctionCall(string functionName, List<Expression> arguments)
        {
            FunctionName = functionName;
            Arguments = arguments;
        }

        public override InterpreterValue Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}