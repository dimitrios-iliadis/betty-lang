using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class UnaryOperatorExpression : Expression
    {
        public Token Operator { get; }
        public Expression Expression { get; }

        public UnaryOperatorExpression(Token @operator, Expression expression)
        {
            Operator = @operator;
            Expression = expression;
        }

        public override Value Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}