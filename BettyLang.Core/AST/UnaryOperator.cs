namespace BettyLang.Core.AST
{
    public class UnaryOperator : AST
    {
        public Token Operator { get; }
        public AST Expression { get; }

        public UnaryOperator(Token op, AST expression)
        {
            Operator = op;
            Expression = expression;
        }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}