namespace BettyLang.Tests.InterpreterTests
{
    public class ConditionalStatementTests : InterpreterTestBase
    {
        [Fact]
        public void DoWhileLoop_ExecutesCorrectNumberOfTimes_WithContinueStatement()
        {
            var code = @"
                counter = 0;
                i = 0;
                do {
                    counter++;
                    if (counter == 3)
                        continue;
                    i++;
                } while (counter < 5);
                return i;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(4.0, result.AsNumber());
        }

        [Fact]
        public void DoWhileLoop_ExecutesCorrectNumberOfTimes_WithBreakStatement()
        {
            var code = @"
                counter = 0;
                do {
                    counter = counter + 1;
                    if (counter == 5) {
                        break;
                    }
                } while (true);
                return counter;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void DoWhileLoop_ExecutesCorrectNumberOfTimes()
        {
            var code = @"
                counter = 0;
                do {
                    counter = counter + 1;
                } while (counter < 5);
                return counter;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void Function_WithForLoop_ReturnsCorrectValue()
        {
            var customCode = @"
                func myfunc() {
                    counter = 0;
                    for (i = 0; i < 5; i++) {
                        if (counter == 3) {
                            return counter;
                        }
                        counter = counter + 1;
                    }
                }

                func main() {
                    result = myfunc();
                    return result;
                }
            ";
            var interpreter = SetupInterpreterCustom(customCode);
            var result = interpreter.Interpret();
            Assert.Equal(3.0, result.AsNumber());
        }

        [Fact]
        public void ForLoop_WithReturnStatement()
        {
            var code = @"
                for (i = 0; i < 5; i = i + 1) {
                    if (i == 3) {
                        return i;
                    }
                }
                return 0;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(3.0, result.AsNumber());
        }

        [Fact]
        public void ForLoop_ExecutesCorrectNumberOfTimes_WithEmptyConditionAndIncrement()
        {
            var code = @"
                counter = 0;
                for (; ; ) {
                    counter = counter + 1;
                    if (counter == 5) {
                        break;
                    }
                }
                return counter;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void ForLoop_SingleStatement_ExecutesCorrectNumberOfTimes()
        {
            var code = @"
                counter = 0;
                for (i = 0; i < 5; i = i + 1) counter = counter + 1;
                return counter;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void ForLoop_WithContinueStatement()
        {

            var code = @"
                counter = 0;
                for (i = 0; i < 5; i = i + 1) {
                    if (i == 2) {
                        continue;
                    }
                    counter = counter + 1;
                }
                return counter;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(4.0, result.AsNumber());
        }

        [Fact]
        public void WhileLoop_WithContinueStatement()
        {
            var code = @"
                counter = 0;
                i = 0;
                while (i < 5) {
                    i = i + 1;
                    if (i == 2) {
                        continue;
                    }
                    counter = counter + 1;
                }
                return counter;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(4.0, result.AsNumber());
        }

        [Fact]
        public void ForLoop_ExecutesCorrectNumberOfTimes_WithEmptyIncrement()
        {
            var code = @"
                counter = 0;
                for (i = 0; i < 5; ) {
                    counter = counter + 1;
                    i = i + 1;
                }
                return counter;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void ForLoop_ExecutesCorrectNumberOfTimes_WithEmptyCondition()
        {
            var code = @"
                counter = 0;
                for (i = 0; ; i = i + 1) {
                    counter = counter + 1;
                    if (counter == 5) {
                        break;
                    }
                }
                return counter;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }
        
        [Fact]
        public void ForLoop_ShorthandIcrement_ExecutesCorrectNumberOfTimes()
        {
            var code = @"
                counter = 0;
                for (i = 0; i < 5; i++) {
                    counter = counter + 1;
                }
                return counter;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void ForLoop_ExecutesCorrectNumberOfTimes()
        {
            var code = @"
                counter = 0;
                for (i = 0; i < 5; i = i + 1) {
                    counter = counter + 1;
                }
                return counter;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void ForLoop_ExecutesCorrectNumberOfTimes_WithEmptyInitializer()
        {
            var code = @"
                counter = 0;
                for (; counter < 5; counter = counter + 1) {
                    counter = counter + 1;
                }
                return counter;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(6.0, result.AsNumber());
        }

        [Fact]
        public void IfStatement_ExecutesCorrectBranch()
        {
            var code = @"
                    if (true) { return 42; }
                    return 0;
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(42.0, result.AsNumber());
        }

        [Fact]
        public void IfStatement_SingleStatement_ExecutesCorrectBranch()
        {
            var code = @"
                    if (true) return 42;
                    return 0;
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(42.0, result.AsNumber());
        }

        [Fact]
        public void IfStatement_ExecutesCorrectBranch_WithElse()
        {
            var code = @"
                    if (false) { return 42; }
                    else { return 0; }
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(0.0, result.AsNumber());
        }

        [Fact]
        public void IfStatement_SingleStatement_ExecutesCorrectBranch_WithElse()
        {
            var code = @"
                    if (false) return 42;
                    else return 0;
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(0.0, result.AsNumber());
        }

        [Fact]
        public void IfStatement_ExecutesCorrectBranch_WithElseIf()
        {
            var code = @"
                    if (false) { return 42; }
                    elif (true) { return 0; }
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(0.0, result.AsNumber());
        }

        [Fact]
        public void IfStatement_SingleStatement_ExecutesCorrectBranch_WithElseIf()
        {
            var code = @"
                    if (false) return 42;
                    elif (true) return 0;
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(0.0, result.AsNumber());
        }

        [Fact]
        public void IfStatement_ExecutesCorrectBranch_WithElseIf_WithElse()
        {
            var code = @"
                    if (false) { return 42; }
                    elif (false) { return 0; }
                    else { return 1; }
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(1.0, result.AsNumber());
        }

        [Fact]
        public void IfStatement_SingleStatement_ExecutesCorrectBranch_WithElseIf_WithElse()
        {
            var code = @"
                    if (false) return 42;
                    elif (false) return 0;
                    else return 1;
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(1.0, result.AsNumber());
        }

        [Fact]
        public void IfStatement_ExecutesCorrectBranch_WithElseIf_WithElse_WithNestedIf()
        {
            var code = @"
                    if (false) { return 42; }
                    elif (false) { return 0; }
                    elif (true) { if (true) { return 1; } }
                    else { return 2; }
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(1.0, result.AsNumber());
        }

        [Fact]
        public void IfStatement_SingleStatement_ExecutesCorrectBranch_WithElseIf_WithElse_WithNestedIf()
        {
            var code = @"
                    if (false) return 42;
                    elif (false) return 0;
                    elif (true) if (true) return 1;
                    else return 2;
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(1.0, result.AsNumber());
        }

        [Fact]
        public void WhileLoop_ExecutesMultipleTimes()
        {
            var code = @"
                counter = 0;
                while (counter < 5) {
                    counter = counter + 1;
                }
                return counter;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void TernaryOperator_ReturnsCorrectValue()
        {
            var code = "return true ? 1 : 0;";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(1.0, result.AsNumber());
        }

        [Fact]
        public void TernaryOperator_FalseCondition_ReturnsCorrectValue()
        {
            var code = "return false ? 1 : 2;";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(2.0, result.AsNumber());
        }

        [Fact]
        public void NestedTernaryOperator_ReturnsCorrectValue()
        {
            var code = "return (true ? false : true) ? 1 : 2;";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(2.0, result.AsNumber());
        }

        [Fact]
        public void TernaryOperator_WithArithmeticExpressions()
        {
            var code = "return (1 + 1 == 2) ? (3 * 2) : (4 / 2);";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(6.0, result.AsNumber());
        }

        [Fact]
        public void TernaryOperator_WithinArithmeticExpressions()
        {
            var code = "return 10 + (false ? 100 : 200) * 2;";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(410.0, result.AsNumber());
        }

        [Fact]
        public void ComplexNestedTernaryOperator_ReturnsCorrectValue()
        {
            var code = "return (5 > 3 ? 3 < 2 : 4 > 2) ? (7 == 7 ? 9 : 8) : (6 == 6 ? 10 : 11);";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(10.0, result.AsNumber());
        }

        [Fact]
        public void TernaryOperator_WithFunctionCalls()
        {
            var customCode = @"
            func add(a, b) { return a + b; }
            func multiply(a, b) { return a * b; }
            func main() {
                return (2 == 2) ? add(4, 5) : multiply(3, 3);
            }
        ";
            var interpreter = SetupInterpreterCustom(customCode);
            var result = interpreter.Interpret();
            Assert.Equal(9.0, result.AsNumber());
        }

        [Fact]
        public void WhileLoop_SingleStatement_ExecutesMultipleTimes()
        {
            var code = @"
                counter = 0;
                while (counter < 5) counter = counter + 1;
                return counter;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }

        [Fact]
        public void WhileLoop_WithBreakStatement()
        {
            var code = @"
                counter = 0;
                while (true) {
                    counter = counter + 1;
                    if (counter == 5) {
                        break;
                    }
                }
                return counter;
            ";
            var interpreter = SetupInterpreter(code);
            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsNumber());
        }
    }
}