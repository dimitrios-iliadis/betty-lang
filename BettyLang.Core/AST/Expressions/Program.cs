using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class Program(List<string> globals, List<FunctionDefinition> functions) : Expression
    {
        public List<string> Globals { get; } = globals;
        public List<FunctionDefinition> Functions { get; } = functions;

        public override InterpreterResult Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}