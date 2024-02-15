using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class ElementAccessExpression(Expression list, Expression index) : Expression
    {
        public Expression List { get; } = list;
        public Expression Index { get; } = index;

        public override InterpreterResult Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}