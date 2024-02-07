namespace BettyLang.Core.AST
{
    public class CompoundStatement : ASTNode
    {
        public List<ASTNode> Statements { get; set; }

        public CompoundStatement() { Statements = []; }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}