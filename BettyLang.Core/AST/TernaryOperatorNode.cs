namespace BettyLang.Core.AST
{
    public class TernaryOperatorNode : ASTNode
    {
        public ASTNode Condition { get; }
        public ASTNode TrueExpression { get; }
        public ASTNode FalseExpression { get; }

        public TernaryOperatorNode(ASTNode condition, ASTNode trueExpression, ASTNode falseExpression) 
        {
            Condition = condition;
            TrueExpression = trueExpression;
            FalseExpression = falseExpression;
        }

        public override object Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}