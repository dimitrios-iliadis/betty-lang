namespace BettyLang.Core.AST
{
    public class TernaryOperator : AST
    {
        public AST Condition { get; }
        public AST TrueExpression { get; }
        public AST FalseExpression { get; }

        public TernaryOperator(AST condition, AST trueExpression, AST falseExpression) 
        {
            Condition = condition;
            TrueExpression = trueExpression;
            FalseExpression = falseExpression;
        }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}