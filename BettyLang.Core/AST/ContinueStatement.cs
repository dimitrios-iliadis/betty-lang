namespace BettyLang.Core.AST
{
    public class ContinueStatement : ASTNode
    {
        public ContinueStatement() { }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}