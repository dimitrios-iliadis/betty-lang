﻿using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public interface IExpressionVisitor
    {
        Value Visit(NumberLiteral node);
        Value Visit(BooleanLiteral node);
        Value Visit(StringLiteral node);
        Value Visit(BinaryOperatorExpression node);
        Value Visit(TernaryOperatorExpression node);
        Value Visit(UnaryOperatorExpression node);
        Value Visit(Variable node);
        Value Visit(FunctionCall node);
        Value Visit(Program node);
    }
}