using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public interface IAstVisitor
    {
        Value Visit(NumberLiteral node);
        Value Visit(BooleanLiteral node);
        Value Visit(StringLiteral node);
        Value Visit(BinaryOperator node);
        Value Visit(TernaryOperator node);
        Value Visit(UnaryOperator node);
        Value Visit(CompoundStatement node);
        Value Visit(Assignment node);
        Value Visit(Variable node);
        Value Visit(EmptyStatement node);
        Value Visit(FunctionDefinition node);
        Value Visit(FunctionCall node);
        Value Visit(IfStatement node);
        Value Visit(WhileStatement node);
        Value Visit(BreakStatement node);
        Value Visit(ContinueStatement node);
        Value Visit(ReturnStatement node);
        Value Visit(Program node);
    }
}