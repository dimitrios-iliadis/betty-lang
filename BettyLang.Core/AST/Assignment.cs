namespace BettyLang.Core.AST
{
    public class Assignment : ASTNode
    {
        public ASTNode Left { get; }
        public Token Operator { get; }
        public ASTNode Right { get; }

        public Assignment(ASTNode left, Token op, ASTNode right)
        {
            Left = left;
            Operator = op;
            Right = right;
        }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}