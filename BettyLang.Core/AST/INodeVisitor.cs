namespace BettyLang.Core.AST
{
    public interface INodeVisitor
    {
        InterpreterValue Visit(NumberLiteral node);
        InterpreterValue Visit(BooleanLiteral node);
        InterpreterValue Visit(StringLiteral node);
        InterpreterValue Visit(BinaryOperator node);
        InterpreterValue Visit(TernaryOperator node);
        InterpreterValue Visit(UnaryOperator node);
        InterpreterValue Visit(CompoundStatement node);
        InterpreterValue Visit(Assignment node);
        InterpreterValue Visit(Variable node);
        InterpreterValue Visit(EmptyStatement node);
        InterpreterValue Visit(FunctionDefinition node);
        InterpreterValue Visit(FunctionCall node);
        InterpreterValue Visit(IfStatement node);
        InterpreterValue Visit(WhileStatement node);
        InterpreterValue Visit(BreakStatement node);
        InterpreterValue Visit(ContinueStatement node);
        InterpreterValue Visit(ReturnStatement node);
        InterpreterValue Visit(Program node);
    }
}