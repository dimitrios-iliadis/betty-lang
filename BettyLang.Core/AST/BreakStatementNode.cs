namespace BettyLang.Core.AST
{
    public class BreakStatementNode : ASTNode
    {
        public BreakStatementNode() { }

        public override object Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}