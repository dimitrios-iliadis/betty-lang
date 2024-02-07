using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class Program : AstNode
    {
        public List<FunctionDefinition> Functions { get; }

        public Program(List<FunctionDefinition> functions)
        {
            Functions = functions;
        }

        public override Value Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}