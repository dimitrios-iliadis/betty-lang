namespace BettyLang.Core.AST
{
    public class CompoundStatementNode : ASTNode
    {
        public List<ASTNode> Statements { get; set; }

        public CompoundStatementNode() { Statements = []; }

        public override object Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}