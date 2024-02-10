
namespace BettyLang.Tests.LexerTests
{
    public class OperatorTests
    {
        [Theory]
        [InlineData("+", TokenType.Plus)]
        [InlineData("-", TokenType.Minus)]
        [InlineData("*", TokenType.Star)]
        [InlineData("/", TokenType.Slash)]
        [InlineData("%", TokenType.Modulo)]
        [InlineData("=", TokenType.Equal)]
        [InlineData("==", TokenType.EqualEqual)]
        [InlineData("!=", TokenType.NotEqual)]
        [InlineData("<", TokenType.LessThan)]
        [InlineData(">", TokenType.GreaterThan)]
        [InlineData("<=", TokenType.LessThanOrEqual)]
        [InlineData(">=", TokenType.GreaterThanOrEqual)]
        [InlineData("&&", TokenType.And)]
        [InlineData("||", TokenType.Or)]
        [InlineData("!", TokenType.Not)]
        public void GetNextToken_HandlesOperatorsCorrectly(string input, TokenType expectedTokenType)
        {
            var lexer = new Lexer(input);

            var token = lexer.GetNextToken();

            Assert.Equal(expectedTokenType, token.Type);
        }
    }
}