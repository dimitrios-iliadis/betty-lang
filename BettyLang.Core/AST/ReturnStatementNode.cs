namespace BettyLang.Core.AST
{
    public class ReturnStatementNode : ASTNode
    {
        public ASTNode ReturnValue { get; }

        public ReturnStatementNode(ASTNode returnValue)
        {
            ReturnValue = returnValue;
        }

        public override object Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}