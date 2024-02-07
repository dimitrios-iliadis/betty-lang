namespace BettyLang.Core.AST
{
    public class EmptyStatement : ASTNode
    {
        public EmptyStatement() { }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}