using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class Variable(string name) : AstNode
    {
        public string Name { get; } = name;

        public override Value Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}