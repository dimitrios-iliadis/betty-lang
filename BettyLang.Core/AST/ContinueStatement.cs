namespace BettyLang.Core.AST
{
    public class ContinueStatement : AST
    {
        public ContinueStatement() { }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}