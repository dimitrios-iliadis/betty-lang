namespace BettyLang.Core.AST
{
    public class BinaryOperator : AST
    {
        public AST Left { get; }
        public Token Operator { get; }
        public AST Right { get; }

        public BinaryOperator(AST left, Token op, AST right)
        {
            Left = left;
            Operator = op;
            Right = right;
        }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}