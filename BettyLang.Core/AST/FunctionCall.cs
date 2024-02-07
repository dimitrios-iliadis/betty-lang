using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class FunctionCall : AstNode
    {
        public string FunctionName { get; }
        public List<AstNode> Arguments { get; }

        public FunctionCall(string functionName, List<AstNode> arguments)
        {
            FunctionName = functionName;
            Arguments = arguments;
        }

        public override Value Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}