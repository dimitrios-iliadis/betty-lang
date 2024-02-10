using BenchmarkDotNet.Attributes;
using BettyLang.Core;

namespace BettyLang.Benchmarks
{
    public class LexerBenchmark
    {
        private const string _input = "func main() { return 1; }";
        private readonly Lexer _lexer = new(_input);

        [Benchmark]
        public void GetAllTokens()
        {
            while (_lexer.GetNextToken().Type != TokenType.EOF)
            {
            }
        }
    }
}