using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class UnaryOperatorExpression(Token op, Expression expression) : Expression
    {
        public Token Operator { get; } = op;
        public Expression Expression { get; } = expression;

        public override InterpreterValue Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}