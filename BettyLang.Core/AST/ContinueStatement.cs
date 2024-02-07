using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class ContinueStatement : AstNode
    {
        public ContinueStatement() { }

        public override Value Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}