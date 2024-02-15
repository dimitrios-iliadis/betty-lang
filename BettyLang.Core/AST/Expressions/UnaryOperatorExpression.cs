using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public enum OperatorFixity
    {
        Prefix,
        Postfix
    }

    public class UnaryOperatorExpression(Expression operand, TokenType op, OperatorFixity fixity) : Expression
    {
        public Expression Operand { get; } = operand;
        public TokenType Operator { get; } = op;
        public OperatorFixity Fixity { get; } = fixity;

        public override InterpreterResult Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}