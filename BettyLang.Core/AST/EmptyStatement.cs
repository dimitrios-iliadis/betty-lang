namespace BettyLang.Core.AST
{
    public class EmptyStatement : Statement
    {
        public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
    }
}