﻿namespace BettyLang.Core.AST
{
    public class Program : ASTNode
    {
        public List<FunctionDefinition> Functions { get; }

        public Program(List<FunctionDefinition> functions)
        {
            Functions = functions;
        }

        public override InterpreterValue Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}