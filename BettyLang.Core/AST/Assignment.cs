using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class Assignment : AstNode
    {
        public AstNode Left { get; }
        public Token Operator { get; }
        public AstNode Right { get; }

        public Assignment(AstNode left, Token @operator, AstNode right)
        {
            Left = left;
            Operator = @operator;
            Right = right;
        }

        public override Value Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}