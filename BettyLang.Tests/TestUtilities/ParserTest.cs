namespace BettyLang.Tests.TestUtilities
{
    public class ParserTest
    {
        protected static Parser SetupParser(string code)
        {
            var lexer = new Lexer(code);
            return new Parser(lexer);
        }
    }
}