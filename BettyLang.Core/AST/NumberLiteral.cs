namespace BettyLang.Core.AST
{
    public class NumberLiteral : ASTNode
    {
        public Token Token { get; }
        public double Value { get; }

        public NumberLiteral(Token token)
        {
            Token = token;
            Value = double.Parse(token.Value);
        }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}