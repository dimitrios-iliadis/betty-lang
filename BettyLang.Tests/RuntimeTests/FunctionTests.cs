namespace BettyLang.Tests.RuntimeTests
{
    public class FunctionTests : InterpreterTest
    {
        [Fact]
        public void SimpleFunction_ReturnsConstantValue()
        {
            var code = @"
                func simple() { return 42; }
                func main() { return simple(); }
            ";
            var interpreter = SetupInterpreterCustom(code);
            var result = interpreter.Interpret();
            Assert.Equal(42, (double)result);
        }

        [Fact]
        public void FunctionWithParameters_CalculatesSum()
        {
            var code = @"
                func sum(a, b) { return a + b; }
                func main() { return sum(5, 7); }
            ";
            var interpreter = SetupInterpreterCustom(code);
            var result = interpreter.Interpret();
            Assert.Equal(12, (double)result);
        }

        [Fact]
        public void RecursiveFunction_CalculatesFactorial()
        {
            var code = @"
                func fact(n) {
                    if (n <= 1) { return 1; }
                    return n * fact(n - 1);
                }
                func main() { return fact(5); }
            ";
            var interpreter = SetupInterpreterCustom(code);
            var result = interpreter.Interpret();
            Assert.Equal(120, (double)result);
        }

        [Fact]
        public void NestedFunctionCalls_WorkCorrectly()
        {
            var code = @"
                func inner(a) { return a * a; }
                func outer(b) { return inner(b) + inner(b + 1); }
                func main() { return outer(3); }
            ";
            var interpreter = SetupInterpreterCustom(code);
            var result = interpreter.Interpret();
            Assert.Equal(9 + 16, (double)result); // 3*3 + 4*4
        }

        [Fact]
        public void FunctionWithLoop_IteratesCorrectly()
        {
            var code = @"
                func sumToN(n) {
                    result = 0;
                    i = 1;
                    while (i <= n) {
                        result = result + i;
                        i = i + 1;
                    }
                    return result;
                }
                func main() { return sumToN(5); }
            ";
            var interpreter = SetupInterpreterCustom(code);
            var result = interpreter.Interpret();
            Assert.Equal(15, (double)result); // Sum of 1 to 5
        }
    }
}