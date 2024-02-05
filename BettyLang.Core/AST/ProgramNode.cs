namespace BettyLang.Core.AST
{
    public class ProgramNode : ASTNode
    {
        public List<FunctionDefinitionNode> Functions { get; }

        public ProgramNode(List<FunctionDefinitionNode> functions)
        {
            Functions = functions;
        }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}