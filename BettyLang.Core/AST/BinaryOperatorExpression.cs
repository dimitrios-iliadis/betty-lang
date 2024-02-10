using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class BinaryOperatorExpression : Expression
    {
        public Expression Left { get; }
        public Token Operator { get; }
        public Expression Right { get; }

        public BinaryOperatorExpression(Expression left, Token @operator, Expression right)
        {
            Left = left;
            Operator = @operator;
            Right = right;
        }

        public override InterpreterValue Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}