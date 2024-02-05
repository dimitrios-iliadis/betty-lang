namespace BettyLang.Core.AST
{
    public class BreakStatementNode : ASTNode
    {
        public BreakStatementNode() { }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}