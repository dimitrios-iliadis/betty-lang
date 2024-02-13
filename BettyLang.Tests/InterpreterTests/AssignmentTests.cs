namespace BettyLang.Tests.InterpreterTests
{
    public class AssignmentTests : InterpreterTestBase
    {
        [Fact]
        public void ChainAssignment_ReturnsCorrectValue()
        {
            var code = "x = y = 5; return x + y;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(10.0, result.AsNumber());
        }

        [Fact]
        public void Assignment_ReturnsCorrectValue()
        {
            var code = "x = 5; return x;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void AssignmentWithArithmeticExpression_ReturnsCorrectValue()
        {
            var code = "x = 2 + 3; return x;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }

        [Theory]
        [InlineData("+=", 9.0)]
        [InlineData("-=", 3.0)]
        [InlineData("*=", 18.0)]
        [InlineData("/=", 2.0)]
        [InlineData("^=", 216.0)]
        [InlineData("%=", 0)]
        public void AssignmentWithShortHandArithmeticExpression_ReturnsCorrectValue(string op, double expected)
        {
            var code = $"x = 6; x {op} 3; return x;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(expected, result.AsNumber());
        }
    }
}