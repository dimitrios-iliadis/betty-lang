namespace BettyLang.Core.AST
{
    public class WhileStatementNode : ASTNode
    {
        public ASTNode Condition { get; private set; }
        public ASTNode Body { get; private set; }

        public WhileStatementNode(ASTNode condition, ASTNode body)
        {
            Condition = condition;
            Body = body;
        }

        public override object Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}