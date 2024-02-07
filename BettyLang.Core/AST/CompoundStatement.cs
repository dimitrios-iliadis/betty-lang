namespace BettyLang.Core.AST
{
    public class CompoundStatement : AST
    {
        public List<AST> Statements { get; set; }

        public CompoundStatement() { Statements = []; }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}