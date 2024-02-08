namespace BettyLang.Core.AST
{
    public class AssignmentStatement : Statement
    {
        public Expression Left { get; }
        public Token Operator { get; }
        public Expression Right { get; }

        public AssignmentStatement(Expression left, Token @operator, Expression right)
        {
            Left = left;
            Operator = @operator;
            Right = right;
        }

        public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
    }
}