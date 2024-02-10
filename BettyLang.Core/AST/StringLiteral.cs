﻿using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class StringLiteral(string value) : Expression
    {
        public string Value { get; } = value;

        public override InterpreterValue Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}