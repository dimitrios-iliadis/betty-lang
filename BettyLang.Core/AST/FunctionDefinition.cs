using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class FunctionDefinition : AstNode
    {
        public string FunctionName { get; }
        public List<string> Parameters { get; }
        public CompoundStatement Body { get; }

        public FunctionDefinition(string functionName, List<string> parameters, CompoundStatement body)
        {
            FunctionName = functionName;
            Parameters = parameters;
            Body = body;
        }

        public override Value Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}