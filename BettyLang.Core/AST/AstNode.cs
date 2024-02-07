using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public abstract class AstNode
    {
        public abstract Value Accept(IAstVisitor visitor);
    }
}
