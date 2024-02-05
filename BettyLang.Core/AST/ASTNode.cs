namespace BettyLang.Core.AST
{
    public abstract class ASTNode
    {
        public abstract InterpreterValue Accept(INodeVisitor visitor);
    }
}