using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class ListValue(List<Expression> elements) : Expression
    {
        public List<Expression> Elements { get; } = elements;

        public override Value Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}