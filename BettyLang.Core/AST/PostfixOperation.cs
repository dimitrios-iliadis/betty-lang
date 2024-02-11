using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class PostfixOperation : Expression
    {
        public Variable Operand { get; }
        public TokenType Operator { get; }

        public PostfixOperation(Variable operand, TokenType op)
        {
            Operand = operand;
            Operator = op;
        }

        public override InterpreterValue Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}