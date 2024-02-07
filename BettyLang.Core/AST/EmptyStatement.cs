using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class EmptyStatement : AstNode
    {
        public EmptyStatement() { }

        public override Value Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}