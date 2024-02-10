using BettyLang.Core.AST;

namespace BettyLang.Core.Interpreter.IntrinsicFunctions
{
    public interface IIntrinsicFunction
    {
        InterpreterValue Invoke(FunctionCall call, IExpressionVisitor visitor);
    }
}