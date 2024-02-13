using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class PrefixOperator(Variable operand, TokenType op) : Expression
    {
        public Variable Operand { get; } = operand;
        public TokenType Operator { get; } = op;

        public override InterpreterValue Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}