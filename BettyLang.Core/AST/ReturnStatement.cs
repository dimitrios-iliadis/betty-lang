using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class ReturnStatement : AstNode
    {
        public AstNode ReturnValue { get; }

        public ReturnStatement(AstNode returnValue)
        {
            ReturnValue = returnValue;
        }

        public override Value Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}