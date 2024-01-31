namespace BettyLang.Core.AST
{
    public interface INodeVisitor
    {
        double Visit(NumberLiteralNode node);
        bool Visit(BooleanLiteralNode node);
        string Visit(StringLiteralNode node);

        object Visit(BinaryOperatorNode node);
        object Visit(TernaryOperatorNode node);
        object Visit(UnaryOperatorNode node);
        object Visit(CompoundStatementNode node);
        object Visit(AssignmentNode node);
        object Visit(VariableNode node);
        object Visit(EmptyStatementNode node);
        object Visit(FunctionDefinitionNode node);
        object Visit(FunctionCallNode node);
        object Visit(IfStatementNode node);
        object Visit(WhileStatementNode node);
        object Visit(BreakStatementNode node);
        object Visit(ContinueStatementNode node);
        object Visit(ReturnStatementNode node);
        object Visit(ProgramNode node);
    }
}