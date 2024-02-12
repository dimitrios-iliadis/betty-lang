namespace BettyLang.Core.AST
{
    public class AssignmentStatement(Expression left, Expression right) : Statement
    {
        public Expression Left { get; } = left;
        public Expression Right { get; } = right;

        public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
    }
}