using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class TernaryOperator : AstNode
    {
        public AstNode Condition { get; }
        public AstNode TrueExpression { get; }
        public AstNode FalseExpression { get; }

        public TernaryOperator(AstNode condition, AstNode trueExpression, AstNode falseExpression) 
        {
            Condition = condition;
            TrueExpression = trueExpression;
            FalseExpression = falseExpression;
        }

        public override Value Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}