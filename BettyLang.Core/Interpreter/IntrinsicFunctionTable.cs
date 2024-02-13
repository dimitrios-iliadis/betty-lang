using BettyLang.Core.AST;
using static BettyLang.Core.Interpreter.IntrinsicFunctions;

namespace BettyLang.Core.Interpreter
{
    public partial class Interpreter
    {
        private delegate InterpreterValue IntrinsicFunctionHandler(FunctionCall call, IExpressionVisitor visitor);

        private static readonly Dictionary<string, IntrinsicFunctionHandler> _intrinsicFunctions = new()
        {
            { "print", PrintFunction },
            { "println", PrintFunction },
            { "input", InputFunction },

            { "tostr", ToStringFunction },
            { "tobool", ToBooleanFunction },
            { "tonum", ToNumberFunction },
            { "tochar", ToCharFunction },

            { "concat", ConcatFunction },
            { "len", LengthFunction },

            { "sin", SinFunction },
            { "cos", CosFunction },
            { "tan", TanFunction },
            { "abs", AbsFunction },
            { "pow", PowFunction },
            { "sqrt", SqrtFunction },
            { "floor", FloorFunction },
            { "ceil", CeilFunction },
        };
    }
}