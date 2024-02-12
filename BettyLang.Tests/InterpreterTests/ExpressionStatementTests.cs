﻿using BettyLang.Core.Interpreter;

namespace BettyLang.Tests.InterpreterTests
{
    public class ExpressionStatementTests : InterpreterTest
    {
        [Fact]
        public void ExpressionStatement_ReturnsNone()
        {
            var code = "x = 5; x;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(InterpreterValue.None(), result);
        }

        [Fact]
        public void ExpressionStatement_PrefixDecrementOperator_WithUnaryMinus_ModifiesVariable()
        {
            var code = "x = 5; - --x + 5; return x;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(4.0, result.AsNumber());
        }
    }
}