using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class BooleanLiteral(bool value) : Expression
    {
        public bool Value { get; } = value;

        public override InterpreterResult Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}