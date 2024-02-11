using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class UnaryOperatorExpression : Expression
    {
        public Token Operator { get; }
        public Expression Expression { get; }

        public UnaryOperatorExpression(Token op, Expression expression)
        {
            Operator = op;
            Expression = expression;
        }

        public override InterpreterValue Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}