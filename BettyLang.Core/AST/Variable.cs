namespace BettyLang.Core.AST
{
    public class Variable : AST
    {
        public Token Token { get; }
        public string Value { get; }

        public Variable(Token token)
        {
            Token = token;
            Value = token.Value;
        }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}