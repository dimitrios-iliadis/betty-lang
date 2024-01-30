﻿using BettyLang.Core.AST;

namespace BettyLang.Core
{
    public class Parser
    {
        private readonly Lexer _lexer;
        private Token _currentToken;
        private HashSet<string> _definedFunctions = new HashSet<string>(); // Store defined function names

        public Parser(Lexer lexer)
        {
            _lexer = lexer;
            _currentToken = lexer.GetNextToken();
        }

        private void Consume(TokenType tokenType)
        {
            if (_currentToken.Type == tokenType)
                _currentToken = _lexer.GetNextToken();
            else
                throw new Exception($"Unexpected token: Expected {tokenType}, found {_currentToken.Type}");
        }

        private ASTNode ParseStringLiteral()
        {
            var token = _currentToken;
            Consume(TokenType.StringLiteral);
            return new StringLiteralNode(token.Value.ToString()!);
        }

        private ASTNode ParseVariable()
        {
            var node = new VariableNode(_currentToken);
            Consume(TokenType.Identifier);
            return node;
        }

        private ASTNode ParseFactor()
        {
            var token = _currentToken;

            switch (token.Type)
            {
                case TokenType.TrueLiteral:
                    Consume(TokenType.TrueLiteral);
                    return new BooleanLiteralNode(true);

                case TokenType.FalseLiteral:
                    Consume(TokenType.FalseLiteral);
                    return new BooleanLiteralNode(false);

                case TokenType.Plus:
                case TokenType.Minus:
                case TokenType.Not:
                    Consume(token.Type);
                    return new UnaryOperatorNode(token, ParseFactor());

                case TokenType.NumberLiteral:
                    Consume(TokenType.NumberLiteral);
                    return new NumberLiteralNode(token);

                case TokenType.LParen:
                    Consume(TokenType.LParen);
                    var node = ParseExpression();
                    Consume(TokenType.RParen);
                    return node;

                case TokenType.Identifier:
                    // Peek at the next token to distinguish between variable and function call
                    var lookahead = _lexer.PeekNextToken();
                    if (lookahead.Type == TokenType.LParen)
                    {
                        // Function call
                        return ParseFunctionCall();
                    }
                    else
                    {
                        // Variable
                        Consume(TokenType.Identifier);
                        return new VariableNode(token);
                    }

                default:
                    throw new Exception($"Unexpected token: {token.Type}");
            }
        }

        private ASTNode ParseTerm()
        {
            if (_currentToken.Type == TokenType.StringLiteral)
                return ParseStringLiteral();

            var node = ParseExponent();

            while (_currentToken.Type == TokenType.Mul || _currentToken.Type == TokenType.Div
                || _currentToken.Type == TokenType.Mod)
            {
                var token = _currentToken;
                if (token.Type == TokenType.Mul)
                    Consume(TokenType.Mul);
                else if (token.Type == TokenType.Div)
                    Consume(TokenType.Div);
                else if (token.Type == TokenType.Mod)
                    Consume(TokenType.Mod);

                node = new BinaryOperatorNode(node, token, ParseExponent());
            }

            return node;
        }

        private ASTNode ParseExponent()
        {
            var node = ParseFactor();

            while (_currentToken.Type == TokenType.Caret)
            {
                var token = _currentToken;
                Consume(TokenType.Caret);
                node = new BinaryOperatorNode(node, token, ParseExponent());
            }

            return node;
        }

        public ASTNode ParseExpression()
        {
            var node = ParseLogicalOrExpression();

            while (_currentToken.Type == TokenType.QuestionMark)
            {
                Consume(TokenType.QuestionMark);
                var trueExpression = ParseExpression();
                Consume(TokenType.Colon);
                var falseExpression = ParseExpression();
                node = new TernaryOperatorNode(condition: node, trueExpression, falseExpression);
            }

            return node;
        }

        private CompoundStatementNode ParseCompoundStatement()
        {
            Consume(TokenType.LBrace);
            var nodes = ParseStatementList();
            Consume(TokenType.RBrace);

            var root = new CompoundStatementNode();
            foreach (var node in nodes)
                root.Statements.Add(node);

            return root;
        }

        private List<ASTNode> ParseStatementList()
        {
            var results = new List<ASTNode>();

            while (_currentToken.Type != TokenType.RBrace && _currentToken.Type != TokenType.EOF)
            {
                var node = ParseStatement();
                results.Add(node);
            }

            return results;
        }

        private ASTNode ParseIfStatement()
        {
            Consume(TokenType.If);
            Consume(TokenType.LParen);
            var condition = ParseExpression();
            Consume(TokenType.RParen);

            // Parse thenStatement as either a compound statement or a single statement
            var thenStatement = (_currentToken.Type == TokenType.LBrace) ? ParseCompoundStatement() : ParseStatement();

            var elseIfStatements = new List<(ASTNode Condition, ASTNode Statement)>();
            ASTNode elseStatement = null;

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

            return new IfStatementNode(condition, thenStatement, elseIfStatements, elseStatement);
        }

        private ASTNode ParseWhileStatement()
        {
            Consume(TokenType.While);
            Consume(TokenType.LParen);
            var condition = ParseExpression();
            Consume(TokenType.RParen);
            var body = (_currentToken.Type == TokenType.LBrace) ? ParseCompoundStatement() : ParseStatement();
            return new WhileStatementNode(condition, body);
        }

        private ASTNode ParseBreakStatement()
        {
            Consume(TokenType.Break);
            Consume(TokenType.Semicolon);
            return new BreakStatementNode();
        }

        private ASTNode ParseContinueStatement()
        {
            Consume(TokenType.Continue);
            Consume(TokenType.Semicolon); // Consume semicolon here
            return new ContinueStatementNode();
        }

        private ASTNode ParseReturnStatement()
        {
            Consume(TokenType.Return);
            ASTNode returnValue = null;
            if (_currentToken.Type != TokenType.Semicolon)
            {
                returnValue = ParseExpression();
            }
            Consume(TokenType.Semicolon);
            return new ReturnStatementNode(returnValue);
        }

        private ASTNode ParseFunctionCallStatement()
        {
            var functionCallNode = ParseFunctionCall() as FunctionCallNode;
            Consume(TokenType.Semicolon);
            return functionCallNode;
        }


        private ASTNode ParseStatement()
        {
            return _currentToken.Type switch
            {
                TokenType.LBrace => ParseCompoundStatement(),
                TokenType.If => ParseIfStatement(),
                TokenType.While => ParseWhileStatement(),
                TokenType.Break => ParseBreakStatement(),
                TokenType.Continue => ParseContinueStatement(),
                TokenType.Return => ParseReturnStatement(),
                TokenType.Identifier => ParseIdentifierStatement(),
                _ => ParseEmptyStatement()
            };
        }

        private ASTNode ParseIdentifierStatement()
        {
            var lookahead = _lexer.PeekNextToken();
            if (lookahead.Type == TokenType.Assign)
            {
                return ParseAssignmentStatement();
            }
            else if (lookahead.Type == TokenType.LParen)
            {
                return ParseFunctionCallStatement();
            }
            else
            {
                throw new Exception($"Unexpected token after identifier: {lookahead.Type}");
            }
        }


        private ASTNode ParseAssignmentStatement()
        {
            var left = ParseVariable();
            var token = _currentToken;
            Consume(TokenType.Assign);
            var right = ParseExpression();
            Consume(TokenType.Semicolon);
            return new AssignmentNode(left, token, right);
        }

        private ASTNode ParseEmptyStatement()
        {
            Consume(TokenType.Semicolon);
            return new EmptyStatementNode();
        }

        private ASTNode ParseFunctionCall()
        {
            // The current token is expected to be an identifier (the function name)
            string functionName = _currentToken.Value;
            Consume(TokenType.Identifier);

            // Consume the opening parenthesis
            Consume(TokenType.LParen);

            // Parse the arguments
            var arguments = new List<ASTNode>();
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
            return new FunctionCallNode(functionName, arguments);
        }

        private FunctionDefinitionNode ParseFunctionDefinition()
        {
            // "function" token is already consumed
            string functionName = _currentToken.Value;
            Consume(TokenType.Identifier); // Function name

            // Check for duplicate function definition
            if (_definedFunctions.Contains(functionName))
            {
                throw new Exception($"Function '{functionName}' is already defined.");
            }
            else
            {
                _definedFunctions.Add(functionName); // Add function name to the set
            }

            Consume(TokenType.LParen); // Opening parenthesis
            var parameters = ParseParameters();
            Consume(TokenType.RParen); // Closing parenthesis

            var body = ParseCompoundStatement(); // Function body

            return new FunctionDefinitionNode(functionName, parameters, body);
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
                        string paramName = _currentToken.Value.ToString();
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

        private ASTNode ParseProgram()
        {
            List<FunctionDefinitionNode> functions = new List<FunctionDefinitionNode>();

            while (_currentToken.Type != TokenType.EOF)
            {
                if (_currentToken.Type == TokenType.Function)
                {
                    Consume(TokenType.Function);
                    functions.Add(ParseFunctionDefinition());
                }
                else
                    throw new Exception("Unexpected token: " + _currentToken.Type);
            }

            return new ProgramNode(functions);
        }

        private ASTNode ParseComparisonExpression()
        {
            var node = ParseArithmeticExpression();

            while (IsComparisonOperator(_currentToken.Type))
            {
                var token = _currentToken;
                Consume(token.Type); // Consume the comparison operator
                node = new BinaryOperatorNode(node, token, ParseArithmeticExpression());
            }

            return node;
        }

        private bool IsComparisonOperator(TokenType type)
        {
            return type == TokenType.GreaterThan
                || type == TokenType.LessThan
                || type == TokenType.GreaterThanOrEqual
                || type == TokenType.LessThanOrEqual
                || type == TokenType.Equal
                || type == TokenType.NotEqual;
        }

        private ASTNode ParseLogicalOrExpression()
        {
            var node = ParseLogicalAndExpression();

            while (_currentToken.Type == TokenType.Or)
            {
                var token = _currentToken;
                Consume(token.Type); // Consume the Or operator
                node = new BinaryOperatorNode(node, token, ParseLogicalAndExpression());
            }

            return node;
        }

        private ASTNode ParseLogicalAndExpression()
        {
            var node = ParseComparisonExpression();

            while (_currentToken.Type == TokenType.And)
            {
                var token = _currentToken;
                Consume(token.Type); // Consume the And operator
                node = new BinaryOperatorNode(node, token, ParseComparisonExpression());
            }

            return node;
        }

        private ASTNode ParseArithmeticExpression()
        {
            var node = ParseTerm();

            while (_currentToken.Type == TokenType.Plus || _currentToken.Type == TokenType.Minus)
            {
                var token = _currentToken;
                if (token.Type == TokenType.Plus)
                    Consume(TokenType.Plus);
                else if (token.Type == TokenType.Minus)
                    Consume(TokenType.Minus);

                node = new BinaryOperatorNode(node, token, ParseTerm());
            }

            return node;
        }

        public ASTNode Parse()
        {
            var node = ParseProgram();

            if (_currentToken.Type != TokenType.EOF)
                throw new Exception($"Unexpected token: {_currentToken.Type}");

            return node;
        }
    }
}