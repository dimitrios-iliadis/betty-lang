namespace BettyLang.Core.AST
{
    public class FunctionCall : AST
    {
        public string FunctionName { get; }
        public List<AST> Arguments { get; }

        public FunctionCall(string functionName, List<AST> arguments)
        {
            FunctionName = functionName;
            Arguments = arguments;
        }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}