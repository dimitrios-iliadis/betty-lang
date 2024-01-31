﻿namespace BettyLang.Tests.RuntimeTests
{
    public class LogicalExpressionTests : InterpreterTest
    {
        [Fact]
        public void AndExpression_BothTrue_ReturnsTrue()
        {
            var code = "return true && true;";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.True((bool)result);
        }

        [Fact]
        public void AndExpression_OneFalse_ReturnsFalse()
        {
            var code = "return true && false;";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.False((bool)result);
        }

        [Fact]
        public void OrExpression_BothFalse_ReturnsFalse()
        {
            var code = "return false || false;";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.False((bool)result);
        }

        [Fact]
        public void OrExpression_OneTrue_ReturnsTrue()
        {
            var code = "return false || true;";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.True((bool)result);
        }

        [Fact]
        public void NotExpression_True_ReturnsFalse()
        {
            var code = "return !true;";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.False((bool)result);
        }

        [Fact]
        public void NotExpression_False_ReturnsTrue()
        {
            var code = "return !false;";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.True((bool)result);
        }
    }
}