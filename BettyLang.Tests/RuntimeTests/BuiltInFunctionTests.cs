﻿namespace BettyLang.Tests.RuntimeTests
{
    public class BuiltInFunctionTests : InterpreterTest
    {
        [Fact]
        public void PrintFunction_PrintsCorrectValue()
        {
            var code = "print(2 + 3);";
            var interpreter = SetupInterpreter(code);
            var output = new StringWriter();
            Console.SetOut(output);

            interpreter.Interpret();

            Assert.Equal("5", output.ToString());
        }

        [Fact]
        public void PrintFunction_PrintsCorrectValueWithNewLine()
        {
            var code = """print(2 + 3 + "\n");""";
            var interpreter = SetupInterpreter(code);
            var output = new StringWriter();
            Console.SetOut(output);

            interpreter.Interpret();

            Assert.Equal("5\n", output.ToString());
        }

        [Fact]
        public void PrintFunction_PrintsCorrectValueWithNewLineAndTab()
        {
            var code = """print(2 + 3 + "\n\t");""";
            var interpreter = SetupInterpreter(code);
            var output = new StringWriter();
            Console.SetOut(output);

            interpreter.Interpret();

            Assert.Equal("5\n\t", output.ToString());
        }

        [Fact]
        public void PrintLineFunction_PrintsCorrectValue()
        {
            var code = "println(2 + 3);";
            var interpreter = SetupInterpreter(code);
            var output = new StringWriter();
            Console.SetOut(output);

            interpreter.Interpret();

            var expected = "5" + Environment.NewLine;
            Assert.Equal(expected, output.ToString());
        }

        [Fact]
        public void InputFunction_ReturnsCorrectValue()
        {
            var code = "return input();";
            var interpreter = SetupInterpreter(code);
            var input = new StringReader("5");
            Console.SetIn(input);

            var result = interpreter.Interpret();

            Assert.Equal(5.0, result.AsDouble());
        }

        [Fact]
        public void InputFunction_ReturnsCorrectValueWithPrompt()
        {
            var code = "return input(\"Enter a number: \");";
            var interpreter = SetupInterpreter(code);
            var input = new StringReader("5");
            Console.SetIn(input);

            var result = interpreter.Interpret();

            Assert.Equal(5.0, result.AsDouble());
        }
    }
}