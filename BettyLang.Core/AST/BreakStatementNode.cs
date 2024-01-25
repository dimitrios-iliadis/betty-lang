﻿namespace BettyLang.Core.AST
{
    public class BreakStatementNode : ASTNode
    {
        public BreakStatementNode() { }

        public override InterpreterResult Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}