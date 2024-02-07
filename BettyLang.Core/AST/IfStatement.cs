using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class IfStatement : AstNode
    {
        public AstNode Condition { get; private set; }
        public AstNode ThenStatement { get; private set; }
        public List<(AstNode Condition, AstNode Statement)> ElseIfStatements { get; private set; }
        public AstNode ElseStatement { get; private set; }

        public IfStatement(AstNode condition, AstNode thenStatement,
            List<(AstNode Condition, AstNode Statement)> elseIfStatements, AstNode elseStatement)
        {
            Condition = condition;
            ThenStatement = thenStatement;
            ElseIfStatements = elseIfStatements ?? new List<(AstNode Condition, AstNode Statement)>();
            ElseStatement = elseStatement;
        }

        public override Value Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}