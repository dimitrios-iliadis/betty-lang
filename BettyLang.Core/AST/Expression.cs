using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public abstract class Expression
    {
        public abstract InterpreterValue Accept(IExpressionVisitor visitor);
    }
}