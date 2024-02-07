namespace BettyLang.Core.AST
{
    public class ReturnStatement : ASTNode
    {
        public ASTNode ReturnValue { get; }

        public ReturnStatement(ASTNode returnValue)
        {
            ReturnValue = returnValue;
        }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}