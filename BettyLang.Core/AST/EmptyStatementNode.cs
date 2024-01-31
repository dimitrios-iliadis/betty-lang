namespace BettyLang.Core.AST
{
    public class EmptyStatementNode : ASTNode
    {
        public EmptyStatementNode() { }

        public override object Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}