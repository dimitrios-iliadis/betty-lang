namespace BettyLang.Core.AST
{
    // Represents an expression being used as a statement.
    public class ExpressionStatement(Expression expression) : Statement
    {
        public Expression Expression { get; } = expression;

        public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
    }
}