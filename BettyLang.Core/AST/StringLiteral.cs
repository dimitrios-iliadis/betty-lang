namespace BettyLang.Core.AST
{
    public class StringLiteral : AST
    {
        public string Value { get; }

        public StringLiteral(string value) { Value = value; }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}