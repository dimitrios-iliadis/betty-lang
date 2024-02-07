namespace BettyLang.Core.AST
{
    public class BreakStatement : AST
    {
        public BreakStatement() { }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}