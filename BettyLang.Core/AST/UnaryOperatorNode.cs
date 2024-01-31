namespace BettyLang.Core.AST
{
    public class UnaryOperatorNode : ASTNode
    {
        public Token Operator { get; }
        public ASTNode Expression { get; }

        public UnaryOperatorNode(Token op, ASTNode expression)
        {
            Operator = op;
            Expression = expression;
        }

        public override object Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}