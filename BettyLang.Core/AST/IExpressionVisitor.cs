using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public interface IExpressionVisitor
    {
        InterpreterResult Visit(NumberLiteral node);
        InterpreterResult Visit(BooleanLiteral node);
        InterpreterResult Visit(StringLiteral node);
        InterpreterResult Visit(CharLiteral node);
        InterpreterResult Visit(BinaryOperatorExpression node);
        InterpreterResult Visit(TernaryOperatorExpression node);
        InterpreterResult Visit(UnaryOperatorExpression node);
        InterpreterResult Visit(Variable node);
        InterpreterResult Visit(FunctionCall node);
        InterpreterResult Visit(Program node);
        InterpreterResult Visit(AssignmentExpression node);
        InterpreterResult Visit(ElementAccessExpression node);
        InterpreterResult Visit(ListValue node);
    }
}