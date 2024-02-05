namespace BettyLang.Core.AST
{
    public class StringLiteralNode : ASTNode
    {
        public string Value { get; }

        public StringLiteralNode(string value) { Value = value; }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}