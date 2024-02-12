using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public interface IExpressionVisitor
    {
        InterpreterValue Visit(NumberLiteral node);
        InterpreterValue Visit(BooleanLiteral node);
        InterpreterValue Visit(StringLiteral node);
        InterpreterValue Visit(BinaryOperatorExpression node);
        InterpreterValue Visit(TernaryOperatorExpression node);
        InterpreterValue Visit(UnaryOperatorExpression node);
        InterpreterValue Visit(Variable node);
        InterpreterValue Visit(FunctionCall node);
        InterpreterValue Visit(Program node);
        InterpreterValue Visit(PrefixOperator node);
        InterpreterValue Visit(PostfixOperator node);
    }
}