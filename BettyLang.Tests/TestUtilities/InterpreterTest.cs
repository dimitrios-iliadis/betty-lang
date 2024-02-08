using BettyLang.Core.Interpreter;

namespace BettyLang.Tests.TestUtilities
{
    public class InterpreterTest
    {
        protected static Interpreter SetupInterpreter(string code)
        {
            return new Interpreter(new Parser(new Lexer($"func main() {{ {code} }}")));
        }

        protected static Interpreter SetupInterpreterCustom(string code)
        {
            return new Interpreter(new Parser(new Lexer(code)));
        }
    }
}