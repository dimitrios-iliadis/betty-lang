using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class FunctionCall(string functionName, List<Expression> arguments) : Expression
    {
        public string FunctionName { get; } = functionName;
        public List<Expression> Arguments { get; } = arguments;

        public override InterpreterResult Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}