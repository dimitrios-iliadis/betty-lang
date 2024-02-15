using BettyLang.Core.Interpreter;

namespace BettyLang.Tests.InterpreterTests
{
    public class ListTests : InterpreterTestBase
    {
        [Fact]
        public void NestedListAccess_ReturnsCorrectInnerMember()
        {
            var code = """
            func main()
            {
                outerList = [1, [2, [3, 4]], 5];
                innerList = outerList[1]; # This should be [2, [3, 4]]
                innerInnerList = innerList[1]; # This should be [3, 4]

                # This should return 4, since we're incrementing the first element of the innerInnerList
                return ++innerInnerList[0]; 
            }
            """;

            var interpreter = SetupInterpreterCustom(code);

            var result = interpreter.Interpret();
            Assert.Equal(4.0, result.AsNumber());
        }

        [Fact]
        public void ListConcatenation_CompoundAssignment_ReturnsCorrectValue()
        {
            var code = "x = [ 1, 2, 3 ]; x += [ 4, 5, 6 ]; return x;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            var expected = new List<InterpreterResult>
            {
                InterpreterResult.FromNumber(1),
                InterpreterResult.FromNumber(2),
                InterpreterResult.FromNumber(3),
                InterpreterResult.FromNumber(4),
                InterpreterResult.FromNumber(5),
                InterpreterResult.FromNumber(6)
            };
            Assert.Equal(result.AsList(), expected);
        }

        [Fact]
        public void ListConcatenation_ReturnsCorrectValue()
        {
            var code = "x = [ 1, 2, 3 ] + [ 4, 5, 6 ]; return x;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            var expected = new List<InterpreterResult>
            {
                InterpreterResult.FromNumber(1),
                InterpreterResult.FromNumber(2),
                InterpreterResult.FromNumber(3),
                InterpreterResult.FromNumber(4),
                InterpreterResult.FromNumber(5),
                InterpreterResult.FromNumber(6)
            };
            Assert.Equal(result.AsList(), expected);
        }

        [Fact]
        public void List_CompoundAssignment_ReturnsCorrectValue()
        {
            var code = "x = [ 1, 2, 3 ]; x += 4; return x;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            var expected = new List<InterpreterResult>
            {
                InterpreterResult.FromNumber(1),
                InterpreterResult.FromNumber(2),
                InterpreterResult.FromNumber(3),
                InterpreterResult.FromNumber(4)
            };
            Assert.Equal(result.AsList(), expected);
        }

        [Fact]
        public void AddElementToList_WithPlusOperator_ReturnsCorrectValue()
        {
            var code = "x = [ 1, 2, 3 ]; x = x + 4; return x;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            var expected = new List<InterpreterResult>
            {
                InterpreterResult.FromNumber(1),
                InterpreterResult.FromNumber(2),
                InterpreterResult.FromNumber(3),
                InterpreterResult.FromNumber(4),
            };
            Assert.Equal(result.AsList(), expected);
        }

        [Fact]
        public void ListDeclarationWithDifferentTypes_ReturnsCorrectValue()
        {
            var code = "x = [ 1, \"hello\", 3 ]; return x;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();

            var expected = new List<InterpreterResult>
            {
                InterpreterResult.FromNumber(1),
                InterpreterResult.FromString("hello"),
                InterpreterResult.FromNumber(3)
            };

            Assert.Equal(result.AsList(), expected);
        }

        [Fact]
        public void ListElementAssignment_ReturnsCorrectValue()
        {
            var code = "x = [ 1, 2, 3 ]; x[1] = 5; return x[1];";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(5, result.AsNumber());
        }

        [Fact]
        public void ListAccess()
        {
            var code = "return [ 1, 2, 3 ][1];";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(2, result.AsNumber());
        }

        [Fact]
        public void ListVariableAccess()
        {
            var code = "x = [ 1, 2, 3 ]; return x[1];";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(2, result.AsNumber());
        }

        [Fact]
        public void MultipleListAccess()
        {
            var code = "x = [ 1, 2, 3 ]; return x[1] + x[2];";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(5, result.AsNumber());
        }

        [Fact]
        public void MultipleTypeListAccess()
        {
            var code = "x = [ 1, \"hello\", 3 ]; return x[1] + x[2];";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal("hello3", result.AsString());
        }

        [Fact]
        public void StringAccess()
        {
            var code = "return \"hello\"[1];";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal('e', result.AsChar());
        }

        [Fact]
        public void StringVariableAccess()
        {
            var code = "x = \"hello\"; return x[1];";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal('e', result.AsChar());
        }

        [Fact]
        public void StringVariableAccess_WithExpression()
        {
            var code = "x = \"hello\"; return x[1 + 1];";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal('l', result.AsChar());
        }

        [Fact]
        public void StringVariableAccess_WithVariableExpression()
        {
            var code = "x = \"hello\"; y = 1; return x[y];";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal('e', result.AsChar());
        }

        [Fact]
        public void FunctionCallWithListAccess()
        {
            var code = """
                func foo() {
                    return "hello";
                }
                func main() {
                    return foo()[1];
                }
                """;
            var interpreter = SetupInterpreterCustom(code);

            var result = interpreter.Interpret();
            Assert.Equal('e', result.AsChar());
        }
    }
}