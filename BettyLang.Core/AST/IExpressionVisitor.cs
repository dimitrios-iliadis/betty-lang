using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public interface IExpressionVisitor
    {
        Value Visit(NumberLiteral node);
        Value Visit(BooleanExpression node);
        Value Visit(StringLiteral node);
        Value Visit(CharLiteral node);
        Value Visit(BinaryOperatorExpression node);
        Value Visit(TernaryOperatorExpression node);
        Value Visit(UnaryOperatorExpression node);
        Value Visit(Variable node);
        Value Visit(FunctionCall node);
        Value Visit(Program node);
        Value Visit(AssignmentExpression node);
        Value Visit(IndexerExpression node);
        Value Visit(ListValue node);
    }
}