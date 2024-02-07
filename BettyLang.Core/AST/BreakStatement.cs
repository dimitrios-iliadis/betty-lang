namespace BettyLang.Core.AST
{
    public class BreakStatement : ASTNode
    {
        public BreakStatement() { }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}