namespace BettyLang.Core.AST
{
    public interface IStatementVisitor
    {
        void Visit(IfStatement node);
        void Visit(WhileStatement node);
        void Visit(BreakStatement node);
        void Visit(ContinueStatement node);
        void Visit(ReturnStatement node);
        void Visit(EmptyStatement node);
        void Visit(FunctionDefinition node);
        void Visit(FunctionCallStatement node);
        void Visit(AssignmentStatement node);
        void Visit(CompoundStatement node);
        void Visit(PostfixOperationStatement node);
    }
}