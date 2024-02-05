namespace BettyLang.Core.AST
{
    public class EmptyStatementNode : ASTNode
    {
        public EmptyStatementNode() { }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}