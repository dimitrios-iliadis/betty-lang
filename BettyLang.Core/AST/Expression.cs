using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public abstract class Expression
    {
        public abstract InterpreterResult Accept(IExpressionVisitor visitor);
    }
}