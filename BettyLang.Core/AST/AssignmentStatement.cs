namespace BettyLang.Core.AST
{
    public class AssignmentStatement(Expression left, Token op, Expression right) : Statement
    {
        public Expression Left { get; } = left;
        public Token Operator { get; } = op;
        public Expression Right { get; } = right;

        public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
    }
}