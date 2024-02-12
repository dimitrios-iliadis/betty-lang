﻿using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class NumberLiteral(Token token) : Expression
    {
        public Token Token { get; } = token;
        public double Value { get; } = double.Parse(token.Value!);

        public override InterpreterValue Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}