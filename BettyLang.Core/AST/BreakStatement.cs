using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class BreakStatement : AstNode
    {
        public BreakStatement() { }

        public override Value Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}