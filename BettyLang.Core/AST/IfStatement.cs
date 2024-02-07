namespace BettyLang.Core.AST
{
    public class IfStatement : ASTNode
    {
        public ASTNode Condition { get; private set; }
        public ASTNode ThenStatement { get; private set; }
        public List<(ASTNode Condition, ASTNode Statement)> ElseIfStatements { get; private set; }
        public ASTNode ElseStatement { get; private set; }

        public IfStatement(ASTNode condition, ASTNode thenStatement,
            List<(ASTNode Condition, ASTNode Statement)> elseIfStatements, ASTNode elseStatement)
        {
            Condition = condition;
            ThenStatement = thenStatement;
            ElseIfStatements = elseIfStatements ?? new List<(ASTNode Condition, ASTNode Statement)>();
            ElseStatement = elseStatement;
        }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}