using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class BooleanLiteral(bool value) : AstNode
    {
        public bool Value { get; } = value;

        public override Value Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}