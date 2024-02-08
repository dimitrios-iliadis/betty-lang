namespace BettyLang.Core.AST
{
    public class IfStatement : Statement
    {
        public Expression Condition { get; private set; }
        public Statement ThenStatement { get; private set; }
        public List<(Expression Condition, Statement Statement)> ElseIfStatements { get; private set; }
        public Statement ElseStatement { get; private set; }

        public IfStatement(Expression condition, Statement thenStatement,
            List<(Expression Condition, Statement Statement)> elseIfStatements, Statement elseStatement)
        {
            Condition = condition;
            ThenStatement = thenStatement;
            ElseIfStatements = elseIfStatements ?? new List<(Expression Condition, Statement Statement)>();
            ElseStatement = elseStatement;
        }

        public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
    }
}