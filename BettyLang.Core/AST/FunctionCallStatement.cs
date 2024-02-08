namespace BettyLang.Core.AST
{
    public class FunctionCallStatement(FunctionCall functionCall) : Statement
    {
        public FunctionCall FunctionCall { get; } = functionCall;

        public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
    }
}