namespace BettyLang.Core.AST
{
    public class ContinueStatementNode : ASTNode
    {
        public ContinueStatementNode() { }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}