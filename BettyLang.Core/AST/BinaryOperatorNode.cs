namespace BettyLang.Core.AST
{
    public class BinaryOperatorNode : ASTNode
    {
        public ASTNode Left { get; }
        public Token Operator { get; }
        public ASTNode Right { get; }

        public BinaryOperatorNode(ASTNode left, Token op, ASTNode right)
        {
            Left = left;
            Operator = op;
            Right = right;
        }

        public override object Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}