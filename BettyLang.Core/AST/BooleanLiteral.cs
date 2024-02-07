﻿namespace BettyLang.Core.AST
{
    public class BooleanLiteral : AST
    {
        public bool Value { get; }

        public BooleanLiteral(bool value) { Value = value; }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}