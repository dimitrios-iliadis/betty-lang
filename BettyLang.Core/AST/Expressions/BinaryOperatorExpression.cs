﻿using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class BinaryOperatorExpression(Expression left, TokenType op, Expression right) : Expression
    {
        public Expression Left { get; } = left;
        public TokenType Operator { get; } = op;
        public Expression Right { get; } = right;

        public override InterpreterValue Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}