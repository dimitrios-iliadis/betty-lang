using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class NumberLiteral(double value) : Expression
    {
        public double Value { get; } = value;

        public override Value Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}