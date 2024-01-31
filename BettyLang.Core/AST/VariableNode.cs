namespace BettyLang.Core.AST
{
    public class VariableNode : ASTNode
    {
        public Token Token { get; }
        public string Value { get; }

        public VariableNode(Token token)
        {
            Token = token;
            Value = token.Value;
        }

        public override object Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}