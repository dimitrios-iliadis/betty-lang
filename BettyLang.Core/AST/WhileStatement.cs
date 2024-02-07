using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class WhileStatement : AstNode
    {
        public AstNode Condition { get; private set; }
        public AstNode Body { get; private set; }

        public WhileStatement(AstNode condition, AstNode body)
        {
            Condition = condition;
            Body = body;
        }

        public override Value Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}