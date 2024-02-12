using BettyLang.Core.AST;

namespace BettyLang.Core
{
    public class Parser
    {
        private readonly Lexer _lexer;
        private Token _currentToken;
        private readonly HashSet<string> _definedFunctions = [];

        private static readonly HashSet<TokenType> _comparisonOperators =
        [
            TokenType.GreaterThan,
            TokenType.LessThan,
            TokenType.GreaterThanOrEqual,
            TokenType.LessThanOrEqual,
            TokenType.EqualEqual,
            TokenType.NotEqual
        ];

        private static readonly HashSet<TokenType> _assignmentOperators =
        [
            TokenType.Equal,
            TokenType.PlusEqual,
            TokenType.MinusEqual,
            TokenType.StarEqual,
            TokenType.SlashEqual,
            TokenType.CaretEqual,
            TokenType.ModuloEqual
        ];

        public Parser(Lexer lexer)
        {
            _lexer = lexer;
            _currentToken = _lexer.GetNextToken();
        }

        private void Consume(TokenType tokenType)
        {
            if (_currentToken.Type == tokenType)
                _currentToken = _lexer.GetNextToken();
            else
                throw new Exception($"Unexpected token: Expected {tokenType}, found {_currentToken.Type}");
        }

        private StringLiteral ParseStringLiteral()
        {
            var token = _currentToken;
            Consume(TokenType.StringLiteral);
            return new StringLiteral(token.Value!);
        }

        private Variable ParseVariable()
        {
            var node = new Variable(_currentToken.Value!);
            Consume(TokenType.Identifier);
            return node;
        }

        private Expression ParseFactor()
        {
            var token = _currentToken;

            switch (token.Type)
            {
                case TokenType.True:
                case TokenType.False:
                    Consume(token.Type);
                    return new BooleanLiteral(token.Type == TokenType.True);

                case TokenType.Increment:
                case TokenType.Decrement:
                    var lookahead = _lexer.PeekNextToken();
                    if (lookahead.Type == TokenType.Identifier)
                    {
                        Consume(token.Type);
                        var variable = ParseVariable();
                        return new PrefixOperator(variable, token.Type);
                    }
                    throw new Exception("Expected an identifier after increment/decrement operator.");

                case TokenType.Plus:
                case TokenType.Minus:
                case TokenType.Not:
                    Consume(token.Type);
                    return new UnaryOperatorExpression(token, ParseFactor());

                case TokenType.NumberLiteral:
                    Consume(TokenType.NumberLiteral);
                    return new NumberLiteral(token);

                case TokenType.LParen:
                    Consume(TokenType.LParen);
                    var node = ParseExpression();
                    Consume(TokenType.RParen);
                    return node;

                case TokenType.Identifier:
                    // Peek at the next token to distinguish between variable and function call
                    lookahead = _lexer.PeekNextToken();
                    if (lookahead.Type == TokenType.LParen)
                    {
                        // Function call
                        return ParseFunctionCall();
                    }
                    else
                    {
                        // Variable
                        Consume(TokenType.Identifier);
                        // Check for postfix increment/decrement
                        if (_currentToken.Type == TokenType.Increment || _currentToken.Type == TokenType.Decrement)
                        {
                            var op = _currentToken;
                            Consume(op.Type);
                            return new PostfixOperator(new Variable(token.Value!), op.Type);
                        }
                        return new Variable(token.Value!);
                    }

                default:
                    throw new Exception($"Unexpected token: {token.Type}");
            }
        }

        private Expression ParseTerm()
        {
            if (_currentToken.Type == TokenType.StringLiteral)
                return ParseStringLiteral();

            var node = ParseExponent();

            while (_currentToken.Type == TokenType.Star || _currentToken.Type == TokenType.Slash
                || _currentToken.Type == TokenType.Modulo)
            {
                var token = _currentToken;
                if (token.Type == TokenType.Star)
                    Consume(TokenType.Star);
                else if (token.Type == TokenType.Slash)
                    Consume(TokenType.Slash);
                else if (token.Type == TokenType.Modulo)
                    Consume(TokenType.Modulo);

                node = new BinaryOperatorExpression(node, token.Type, ParseExponent());
            }

            return node;
        }

        private Expression ParseExponent()
        {
            var node = ParseFactor();

            while (_currentToken.Type == TokenType.Caret)
            {
                var token = _currentToken;
                Consume(TokenType.Caret);
                node = new BinaryOperatorExpression(node, token.Type, ParseExponent());
            }

            return node;
        }

        private Expression ParseExpression()
        {
            var node = ParseAssignmentExpression();

            while (_currentToken.Type == TokenType.QuestionMark)
            {
                Consume(TokenType.QuestionMark);
                var trueExpression = ParseExpression();
                Consume(TokenType.Colon);
                var falseExpression = ParseExpression();
                node = new TernaryOperatorExpression(condition: node, trueExpression, falseExpression);
            }

            return node;
        }

        private CompoundStatement ParseCompoundStatement()
        {
            Consume(TokenType.LBrace);
            var nodes = ParseStatementList();
            Consume(TokenType.RBrace);

            var root = new CompoundStatement();
            foreach (var node in nodes)
                root.Statements.Add(node);

            return root;
        }

        private List<Statement> ParseStatementList()
        {
            var results = new List<Statement>();

            while (_currentToken.Type != TokenType.RBrace && _currentToken.Type != TokenType.EOF)
            {
                var node = ParseStatement();
                results.Add(node);
            }

            return results;
        }

        private IfStatement ParseIfStatement()
        {
            Consume(TokenType.If);
            Consume(TokenType.LParen);
            var condition = ParseExpression();
            Consume(TokenType.RParen);

            // Parse thenStatement as either a compound statement or a single statement
            var thenStatement = (_currentToken.Type == TokenType.LBrace) ? ParseCompoundStatement() : ParseStatement();

            var elseIfStatements = new List<(Expression Condition, Statement Statement)>();
            Statement? elseStatement = null;

            while (_currentToken.Type == TokenType.Elif)
            {
                Consume(TokenType.Elif);
                Consume(TokenType.LParen);
                var elseIfCondition = ParseExpression();
                Consume(TokenType.RParen);
                // Parse elseIfStatement similarly
                var elseIfStatement = (_currentToken.Type == TokenType.LBrace) ? ParseCompoundStatement() : ParseStatement();
                elseIfStatements.Add((elseIfCondition, elseIfStatement));
            }

            if (_currentToken.Type == TokenType.Else)
            {
                Consume(TokenType.Else);
                // Parse elseStatement similarly
                elseStatement = (_currentToken.Type == TokenType.LBrace) ? ParseCompoundStatement() : ParseStatement();
            }

            return new IfStatement(condition, thenStatement, elseIfStatements, elseStatement);
        }

        private ForStatement ParseForStatement()
        {
            Consume(TokenType.For);
            Consume(TokenType.LParen);

            Expression? initializer = null;
            if (_currentToken.Type != TokenType.Semicolon)
            {
                initializer = ParseExpression();
            }
            Consume(TokenType.Semicolon);

            Expression? condition = null;
            if (_currentToken.Type != TokenType.Semicolon)
            {
                condition = ParseExpression();
            }
            Consume(TokenType.Semicolon);

            Expression? increment = null;
            if (_currentToken.Type != TokenType.RParen)
            {
                increment = ParseExpression();
            }
            Consume(TokenType.RParen);

            var body = (_currentToken.Type == TokenType.LBrace) ? ParseCompoundStatement() : ParseStatement();

            return new ForStatement(initializer, condition, increment, body);
        }

        private WhileStatement ParseWhileStatement()
        {
            Consume(TokenType.While);
            Consume(TokenType.LParen);
            var condition = ParseExpression();
            Consume(TokenType.RParen);
            var body = (_currentToken.Type == TokenType.LBrace) ? ParseCompoundStatement() : ParseStatement();
            return new WhileStatement(condition, body);
        }

        private BreakStatement ParseBreakStatement()
        {
            Consume(TokenType.Break);
            Consume(TokenType.Semicolon);
            return new BreakStatement();
        }

        private ContinueStatement ParseContinueStatement()
        {
            Consume(TokenType.Continue);
            Consume(TokenType.Semicolon);
            return new ContinueStatement();
        }

        private ReturnStatement ParseReturnStatement()
        {
            Consume(TokenType.Return);
            Expression? returnValue = null;
            if (_currentToken.Type != TokenType.Semicolon)
            {
                returnValue = ParseExpression();
            }
            Consume(TokenType.Semicolon);
            return new ReturnStatement(returnValue);
        }

        private Statement ParseStatement()
        {
            return _currentToken.Type switch
            {
                TokenType.LBrace => ParseCompoundStatement(),
                TokenType.If => ParseIfStatement(),
                TokenType.For => ParseForStatement(),
                TokenType.While => ParseWhileStatement(),
                TokenType.Break => ParseBreakStatement(),
                TokenType.Continue => ParseContinueStatement(),
                TokenType.Return => ParseReturnStatement(),
                TokenType.Semicolon => ParseEmptyStatement(),
                _ => ParseExpressionStatement()
            };
        }

        private ExpressionStatement ParseExpressionStatement()
        {
            // Parse an expression generally
            var expression = ParseExpression();
            Consume(TokenType.Semicolon); // Ensure it ends with a semicolon
            return new ExpressionStatement(expression);
        }

        private Expression ParseAssignmentExpression()
        {
            var leftExpr = ParseLogicalOrExpression(); // Parse the highest precedence expressions first

            if (IsAssignmentOperator(_currentToken.Type))
            {
                var assignmentOperator = _currentToken;
                Consume(assignmentOperator.Type); // Move past the assignment operator

                var rightExpr = ParseAssignmentExpression(); // Parse the right expression recursively, allowing for chained assignments
                rightExpr = assignmentOperator.Type switch
                {
                    TokenType.Equal => new AssignmentExpression(leftExpr, rightExpr),
                    TokenType.PlusEqual => new AssignmentExpression(leftExpr, new BinaryOperatorExpression(leftExpr, TokenType.Plus, rightExpr)),
                    TokenType.MinusEqual => new AssignmentExpression(leftExpr, new BinaryOperatorExpression(leftExpr, TokenType.Minus, rightExpr)),
                    TokenType.StarEqual => new AssignmentExpression(leftExpr, new BinaryOperatorExpression(leftExpr, TokenType.Star, rightExpr)),
                    TokenType.SlashEqual => new AssignmentExpression(leftExpr, new BinaryOperatorExpression(leftExpr, TokenType.Slash, rightExpr)),
                    TokenType.CaretEqual => new AssignmentExpression(leftExpr, new BinaryOperatorExpression(leftExpr, TokenType.Caret, rightExpr)),
                    TokenType.ModuloEqual => new AssignmentExpression(leftExpr, new BinaryOperatorExpression(leftExpr, TokenType.Modulo, rightExpr)),
                    _ => throw new Exception($"Unexpected token: {assignmentOperator.Type}")
                };

                return new AssignmentExpression(leftExpr, rightExpr); // Construct an assignment expression
            }

            return leftExpr; // If no assignment operator is found, return the left expression
        }

        private static bool IsAssignmentOperator(TokenType type) => _assignmentOperators.Contains(type);

        private EmptyStatement ParseEmptyStatement()
        {
            Consume(TokenType.Semicolon);
            return new EmptyStatement();
        }

        private FunctionCall ParseFunctionCall()
        {
            // The current token is expected to be an identifier (the function name)
            string functionName = _currentToken.Value!;
            Consume(TokenType.Identifier);

            // Consume the opening parenthesis
            Consume(TokenType.LParen);

            // Parse the arguments
            var arguments = new List<Expression>();
            if (_currentToken.Type != TokenType.RParen) // Check if the next token is not a right parenthesis
            {
                do
                {
                    if (_currentToken.Type == TokenType.Comma)
                        Consume(TokenType.Comma); // Consume the comma before parsing the next argument

                    arguments.Add(ParseExpression());
                }
                while (_currentToken.Type == TokenType.Comma); // Continue if there's a comma (more arguments)
            }

            // Consume the closing parenthesis
            Consume(TokenType.RParen);

            // Return a new function call node
            return new FunctionCall(functionName, arguments);
        }

        private FunctionDefinition ParseFunctionDefinition()
        {
            // "function" token is already consumed
            string functionName = _currentToken.Value!;
            Consume(TokenType.Identifier); // Function name

            // Check for duplicate function definition
            if (!_definedFunctions.Add(functionName)) // Try to add the function name to the set
                throw new Exception($"Function '{functionName}' is already defined.");

            Consume(TokenType.LParen); // Opening parenthesis
            var parameters = ParseParameters();
            Consume(TokenType.RParen); // Closing parenthesis

            var body = ParseCompoundStatement(); // Function body

            return new FunctionDefinition(functionName, parameters, body);
        }

        private List<string> ParseParameters()
        {
            var parameters = new List<string>();

            if (_currentToken.Type != TokenType.RParen) // Check if parameter list is empty
            {
                do
                {
                    if (_currentToken.Type == TokenType.Identifier)
                    {
                        string paramName = _currentToken.Value!.ToString();
                        parameters.Add(paramName);
                        Consume(TokenType.Identifier);
                    }
                    else
                    {
                        throw new Exception($"Expected an identifier, found {_currentToken.Type}.");
                    }

                    if (_currentToken.Type == TokenType.Comma)
                    {
                        Consume(TokenType.Comma); // Eat comma and expect another parameter
                    }
                }
                while (_currentToken.Type != TokenType.RParen);
            }

            return parameters;
        }

        private Program ParseProgram()
        {
            var functions = new List<FunctionDefinition>();

            while (_currentToken.Type != TokenType.EOF)
            {
                if (_currentToken.Type == TokenType.Func)
                {
                    Consume(TokenType.Func);
                    functions.Add(ParseFunctionDefinition());
                }
                else
                    throw new Exception("Unexpected token: " + _currentToken.Type);
            }

            return new Program(functions);
        }

        private Expression ParseComparisonExpression()
        {
            var node = ParseArithmeticExpression();

            while (IsComparisonOperator(_currentToken.Type))
            {
                var token = _currentToken;
                Consume(token.Type); // Consume the comparison operator
                node = new BinaryOperatorExpression(node, token.Type, ParseArithmeticExpression());
            }

            return node;
        }

        private static bool IsComparisonOperator(TokenType type) => _comparisonOperators.Contains(type);

        private Expression ParseLogicalOrExpression()
        {
            var node = ParseLogicalAndExpression();

            while (_currentToken.Type == TokenType.Or)
            {
                var token = _currentToken;
                Consume(token.Type); // Consume the Or operator
                node = new BinaryOperatorExpression(node, token.Type, ParseLogicalAndExpression());
            }

            return node;
        }

        private Expression ParseLogicalAndExpression()
        {
            var node = ParseComparisonExpression();

            while (_currentToken.Type == TokenType.And)
            {
                var token = _currentToken;
                Consume(token.Type); // Consume the And operator
                node = new BinaryOperatorExpression(node, token.Type, ParseComparisonExpression());
            }

            return node;
        }

        private Expression ParseArithmeticExpression()
        {
            var node = ParseTerm();

            while (_currentToken.Type == TokenType.Plus || _currentToken.Type == TokenType.Minus)
            {
                var token = _currentToken;
                if (token.Type == TokenType.Plus)
                    Consume(TokenType.Plus);
                else if (token.Type == TokenType.Minus)
                    Consume(TokenType.Minus);

                node = new BinaryOperatorExpression(node, token.Type, ParseTerm());
            }

            return node;
        }

        public Expression Parse()
        {
            var node = ParseProgram();

            if (_currentToken.Type != TokenType.EOF)
                throw new Exception($"Unexpected token: {_currentToken.Type}");

            return node;
        }
    }
}