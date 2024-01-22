﻿namespace BettyLang.Core.AST
{
    public class BooleanLiteralNode : ASTNode
    {
        public bool Value { get; }

        public BooleanLiteralNode(bool value) { Value = value; }

        public override InterpreterResult Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}