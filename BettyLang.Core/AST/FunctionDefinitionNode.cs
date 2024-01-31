namespace BettyLang.Core.AST
{
    public class FunctionDefinitionNode : ASTNode
    {
        public string FunctionName { get; }
        public List<string> Parameters { get; }
        public CompoundStatementNode Body { get; }

        public FunctionDefinitionNode(string functionName, List<string> parameters, CompoundStatementNode body)
        {
            FunctionName = functionName;
            Parameters = parameters;
            Body = body;
        }

        public override object Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}