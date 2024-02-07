using BenchmarkDotNet.Attributes;
using BettyLang.Core;

namespace BettyLang.Benchmarks
{
    public class InterpreterBenchmarks
    {
        [Benchmark]
        public void RecursiveFunctionCall()
        {
            var code = $$"""
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
            var interpreter = new Interpreter(parser);

            interpreter.Interpret();
        }
    }
}