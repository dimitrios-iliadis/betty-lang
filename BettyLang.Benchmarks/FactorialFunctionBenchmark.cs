using BenchmarkDotNet.Attributes;
using BettyLang.Core;
using BettyLang.Core.Interpreter;

namespace BettyLang.Benchmarks
{
    public class FactorialFunctionBenchmark
    {
        private Interpreter interpreter = null!;

        [GlobalSetup]
        public void Setup()
        {
            string code = """
            func fact(n)
            {
                if (n == 0)
                {
                    return 1;
                }
                else
                {
                    return n * fact(n - 1);
                }
            }

            func main()
            {
                fact(5);
            }
            """;

            var lexer = new Lexer(code);
            var parser = new Parser(lexer);
            interpreter = new Interpreter(parser);
        }

        [Benchmark]
        public void BenchmarkFactorialFunction()
        {
            interpreter.Interpret();
        }
    }
}