using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class Variable(string name) : Expression
    {
        public string Name { get; } = name;

        public override InterpreterValue Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}