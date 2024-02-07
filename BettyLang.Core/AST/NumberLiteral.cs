using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class NumberLiteral : AstNode
    {
        public Token Token { get; }
        public double Value { get; }

        public NumberLiteral(Token token)
        {
            Token = token;
            Value = double.Parse(token.Value);
        }

        public override Value Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}