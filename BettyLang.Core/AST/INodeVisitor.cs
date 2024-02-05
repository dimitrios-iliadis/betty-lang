namespace BettyLang.Core.AST
{
    public interface INodeVisitor
    {
        InterpreterValue Visit(NumberLiteralNode node);
        InterpreterValue Visit(BooleanLiteralNode node);
        InterpreterValue Visit(StringLiteralNode node);
        InterpreterValue Visit(BinaryOperatorNode node);
        InterpreterValue Visit(TernaryOperatorNode node);
        InterpreterValue Visit(UnaryOperatorNode node);
        InterpreterValue Visit(CompoundStatementNode node);
        InterpreterValue Visit(AssignmentNode node);
        InterpreterValue Visit(VariableNode node);
        InterpreterValue Visit(EmptyStatementNode node);
        InterpreterValue Visit(FunctionDefinitionNode node);
        InterpreterValue Visit(FunctionCallNode node);
        InterpreterValue Visit(IfStatementNode node);
        InterpreterValue Visit(WhileStatementNode node);
        InterpreterValue Visit(BreakStatementNode node);
        InterpreterValue Visit(ContinueStatementNode node);
        InterpreterValue Visit(ReturnStatementNode node);
        InterpreterValue Visit(ProgramNode node);
    }
}