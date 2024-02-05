namespace BettyLang.Core.AST
{
    public class ReturnStatementNode : ASTNode
    {
        public ASTNode ReturnValue { get; }

        public ReturnStatementNode(ASTNode returnValue)
        {
            ReturnValue = returnValue;
        }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}