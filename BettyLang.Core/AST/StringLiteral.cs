using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class StringLiteral : AstNode
    {
        public string Value { get; }

        public StringLiteral(string value) { Value = value; }

        public override Value Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}