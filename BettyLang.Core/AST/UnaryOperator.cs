namespace BettyLang.Core.AST
{
    public class UnaryOperator : ASTNode
    {
        public Token Operator { get; }
        public ASTNode Expression { get; }

        public UnaryOperator(Token op, ASTNode expression)
        {
            Operator = op;
            Expression = expression;
        }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}