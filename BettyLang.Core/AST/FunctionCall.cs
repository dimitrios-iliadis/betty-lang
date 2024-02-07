namespace BettyLang.Core.AST
{
    public class FunctionCall : ASTNode
    {
        public string FunctionName { get; }
        public List<ASTNode> Arguments { get; }

        public FunctionCall(string functionName, List<ASTNode> arguments)
        {
            FunctionName = functionName;
            Arguments = arguments;
        }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}