using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class Program(List<FunctionDefinition> functions) : Expression
    {
        public List<FunctionDefinition> Functions { get; } = functions;

        public override Value Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}