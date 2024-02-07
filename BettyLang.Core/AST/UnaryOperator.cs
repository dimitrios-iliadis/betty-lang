using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class UnaryOperator : AstNode
    {
        public Token Operator { get; }
        public AstNode Expression { get; }

        public UnaryOperator(Token op, AstNode expression)
        {
            Operator = op;
            Expression = expression;
        }

        public override Value Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}