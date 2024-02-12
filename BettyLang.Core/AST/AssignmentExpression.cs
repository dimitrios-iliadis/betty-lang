using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class AssignmentExpression(Expression lhs, Expression rhs) : Expression
    {
        public Expression LHS { get; } = lhs;
        public Expression RHS { get; } = rhs;

        public override InterpreterValue Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}