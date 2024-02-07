namespace BettyLang.Core.AST
{
    public class WhileStatement : AST
    {
        public AST Condition { get; private set; }
        public AST Body { get; private set; }

        public WhileStatement(AST condition, AST body)
        {
            Condition = condition;
            Body = body;
        }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}