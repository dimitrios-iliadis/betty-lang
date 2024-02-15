namespace BettyLang.Tests.InterpreterTests
{
    public class GlobalVariableTests : InterpreterTestBase
    {
        [Fact]
        public void GlobalVariable_CanBeAssignedAndRead()
        {
            var code = """
                global x;

                func main() {
                    x = 5;
                    return x;
                }
                """;
            var interpreter = SetupInterpreterCustom(code);

            var result = interpreter.Interpret();

            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void GlobalVariable_CanBeAssignedAndReadInMultipleFunctions()
        {
            var code = """
                global x;

                func main() {
                    x = 5;
                    other();
                    return x;
                }

                func other() {
                    x = 3;
                }
                """;
            var interpreter = SetupInterpreterCustom(code);

            var result = interpreter.Interpret();

            Assert.Equal(3.0, result.AsNumber());
        }
    }
}