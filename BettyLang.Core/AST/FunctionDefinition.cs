﻿namespace BettyLang.Core.AST
{
    public class FunctionDefinition : Statement
    {
        public string FunctionName { get; }
        public List<string> Parameters { get; }
        public CompoundStatement Body { get; }

        public FunctionDefinition(string functionName, List<string> parameters, CompoundStatement body)
        {
            FunctionName = functionName;
            Parameters = parameters;
            Body = body;
        }

        public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
    }
}