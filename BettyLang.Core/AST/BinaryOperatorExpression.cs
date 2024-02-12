using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class BinaryOperatorExpression(Expression left, Token op, Expression right) : Expression
    {
        public Expression Left { get; } = left;
        public Token Operator { get; } = op;
        public Expression Right { get; } = right;

        public override InterpreterValue Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}