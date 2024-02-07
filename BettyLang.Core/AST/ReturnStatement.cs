namespace BettyLang.Core.AST
{
    public class ReturnStatement : AST
    {
        public AST ReturnValue { get; }

        public ReturnStatement(AST returnValue)
        {
            ReturnValue = returnValue;
        }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}