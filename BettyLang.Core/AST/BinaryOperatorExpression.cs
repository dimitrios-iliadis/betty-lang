using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class BinaryOperatorExpression : Expression
    {
        public Expression Left { get; }
        public Token Operator { get; }
        public Expression Right { get; }

        public BinaryOperatorExpression(Expression left, Token op, Expression right)
        {
            Left = left;
            Operator = op;
            Right = right;
        }

        public override InterpreterValue Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}