using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class StringLiteral(string value) : Expression
    {
        public string Value { get; } = value;

        public override InterpreterResult Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}