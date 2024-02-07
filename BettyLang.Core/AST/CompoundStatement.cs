using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class CompoundStatement : AstNode
    {
        public List<AstNode> Statements { get; set; }

        public CompoundStatement() { Statements = []; }

        public override Value Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}