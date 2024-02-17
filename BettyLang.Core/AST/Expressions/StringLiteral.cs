using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class StringLiteral(string value) : Expression
    {
        public string Value { get; } = value;

        public override Value Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}