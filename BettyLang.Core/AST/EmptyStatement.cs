namespace BettyLang.Core.AST
{
    public class EmptyStatement : AST
    {
        public EmptyStatement() { }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}