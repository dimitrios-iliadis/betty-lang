namespace BettyLang.Core.AST
{
    public class PostfixOperationStatement(PostfixOperation postfixOperation) : Statement
    {
        public PostfixOperation PostfixOperation { get; } = postfixOperation;

        public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
    }
}