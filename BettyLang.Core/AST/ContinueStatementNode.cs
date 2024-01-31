namespace BettyLang.Core.AST
{
    public class ContinueStatementNode : ASTNode
    {
        public ContinueStatementNode() { }

        public override object Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}