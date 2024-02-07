namespace BettyLang.Core.AST
{
    public class TernaryOperator : ASTNode
    {
        public ASTNode Condition { get; }
        public ASTNode TrueExpression { get; }
        public ASTNode FalseExpression { get; }

        public TernaryOperator(ASTNode condition, ASTNode trueExpression, ASTNode falseExpression) 
        {
            Condition = condition;
            TrueExpression = trueExpression;
            FalseExpression = falseExpression;
        }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}