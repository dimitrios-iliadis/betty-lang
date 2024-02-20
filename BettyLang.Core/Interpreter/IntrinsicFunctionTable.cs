using BettyLang.Core.AST;
using static BettyLang.Core.Interpreter.IntrinsicFunctions;

namespace BettyLang.Core.Interpreter
{
    public partial class Interpreter
    {
        private delegate Value IntrinsicFunctionHandler(FunctionCall call, IExpressionVisitor visitor);

        private static readonly Dictionary<string, IntrinsicFunctionHandler> _intrinsicFunctions = new()
        {
            { "print", PrintFunction },
            { "println", PrintFunction },
            { "input", InputFunction },

            { "tostr", ToStringFunction },
            { "tobool", ToBooleanFunction },
            { "tonum", ToNumberFunction },
            { "tochar", ToCharFunction },
            { "tolist", ToListFunction },

            { "concat", ConcatFunction },

            { "isdigit", IsDigitFunction },
            { "isspace", IsSpaceFunction },

            { "len", LengthFunction },

            { "append", AppendFunction },
            { "range", RangeFunction },
            { "remove", RemoveFunction },
            { "removeat", RemoveAtFunction },

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