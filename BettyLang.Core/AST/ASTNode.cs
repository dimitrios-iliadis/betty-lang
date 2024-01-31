namespace BettyLang.Core.AST
{
    public abstract class ASTNode
    {
        public abstract object Accept(INodeVisitor visitor);
    }
}