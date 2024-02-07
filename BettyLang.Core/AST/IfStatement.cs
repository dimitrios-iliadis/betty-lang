namespace BettyLang.Core.AST
{
    public class IfStatement : AST
    {
        public AST Condition { get; private set; }
        public AST ThenStatement { get; private set; }
        public List<(AST Condition, AST Statement)> ElseIfStatements { get; private set; }
        public AST ElseStatement { get; private set; }

        public IfStatement(AST condition, AST thenStatement,
            List<(AST Condition, AST Statement)> elseIfStatements, AST elseStatement)
        {
            Condition = condition;
            ThenStatement = thenStatement;
            ElseIfStatements = elseIfStatements ?? new List<(AST Condition, AST Statement)>();
            ElseStatement = elseStatement;
        }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}