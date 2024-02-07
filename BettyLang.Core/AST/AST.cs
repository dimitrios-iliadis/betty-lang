namespace BettyLang.Core.AST
{
    public abstract class AST
    {
        public abstract InterpreterValue Accept(INodeVisitor visitor);
    }
}