﻿using BettyLang.Core.AST;

namespace BettyLang.Core
{
    public class Parser
    {
        private readonly Lexer _lexer;
        private Token _currentToken;

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

        private ASTNode ParseString()
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

            if (token.Type == TokenType.TrueLiteral)
            {
                Consume(TokenType.TrueLiteral);
                var node = new BooleanLiteralNode(true);
                return node;
            }
            else if (token.Type == TokenType.FalseLiteral)
            {
                Consume(TokenType.FalseLiteral);
                var node = new BooleanLiteralNode(false);
                return node;
            }
            else if (token.Type == TokenType.Plus)
            {
                Consume(TokenType.Plus);
                var node = new UnaryOperatorNode(token, ParseFactor());
                return node;
            }
            else if (token.Type == TokenType.Minus)
            {
                Consume(TokenType.Minus);
                var node = new UnaryOperatorNode(token, ParseFactor());
                return node;
            }
            else if (token.Type == TokenType.Not)
            {
                Consume(TokenType.Not);
                var node = new UnaryOperatorNode(token, ParseFactor());
                return node;
            }
            else if (token.Type == TokenType.NumberLiteral)
            {
                Consume(TokenType.NumberLiteral);
                return new NumberLiteralNode(token);
            }
            else if (token.Type == TokenType.LParen)
            {
                Consume(TokenType.LParen);
                var node = ParseExpression();
                Consume(TokenType.RParen);
                return node;
            }
            else
            {
                var node = ParseVariable();
                return node;
            }
        }

        private ASTNode ParseTerm()
        {
            if (_currentToken.Type == TokenType.StringLiteral)
                return ParseString();

            var node = ParseExponent();

            while (_currentToken.Type == TokenType.Mul || _currentToken.Type == TokenType.Div)
            {
                var token = _currentToken;
                if (token.Type == TokenType.Mul)
                    Consume(TokenType.Mul);
                else if (token.Type == TokenType.Div)
                    Consume(TokenType.Div);

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
            return ParseOrExpression();
        }

        private CompoundStatementNode ParseCompoundStatement()
        {
            Consume(TokenType.LBracket);
            var nodes = ParseStatementList();
            Consume(TokenType.RBracket);

            var root = new CompoundStatementNode();
            foreach (var node in nodes)
                root.Statements.Add(node);

            return root;
        }

        private bool RequiresSemicolon(ASTNode node)
        {
            return !(node is CompoundStatementNode
                || node is IfStatementNode
                || node is WhileStatementNode);
        }

        private List<ASTNode> ParseStatementList()
        {
            var results = new List<ASTNode>();

            while (_currentToken.Type != TokenType.RBracket && _currentToken.Type != TokenType.EOF)
            {
                var node = ParseStatement();
                results.Add(node);

                if (RequiresSemicolon(node))
                {
                    // Expect a semicolon after simple statements
                    if (_currentToken.Type == TokenType.Semicolon)
                        Consume(TokenType.Semicolon);
                    else
                        throw new Exception($"Expected semicolon after statement, found '{_currentToken.Type}'");
                }
            }

            return results;
        }

        private ASTNode ParseInputStatement()
        {
            Consume(TokenType.Input);

            if (_currentToken.Type != TokenType.Identifier)
                throw new Exception("Expected variable name after 'input'");

            string variableName = _currentToken.Value.ToString();
            Consume(TokenType.Identifier);

            return new InputStatementNode(variableName);
        }

        private ASTNode ParsePrintStatement()
        {
            Consume(TokenType.Print);
            ASTNode expression = ParseExpression();
            return new PrintStatementNode(expression);
        }

        private ASTNode ParseIfStatement()
        {
            Consume(TokenType.If);
            Consume(TokenType.LParen);
            var condition = ParseExpression();
            Consume(TokenType.RParen);
            var thenStatement = ParseCompoundStatement();

            var elseIfStatements = new List<(ASTNode Condition, ASTNode Statement)>();
            ASTNode elseStatement = null;

            while (_currentToken.Type == TokenType.Elif)
            {
                Consume(TokenType.Elif);
                Consume(TokenType.LParen);
                var elseIfCondition = ParseExpression();
                Consume(TokenType.RParen);
                var elseIfStatement = ParseCompoundStatement();
                elseIfStatements.Add((elseIfCondition, elseIfStatement));
            }

            if (_currentToken.Type == TokenType.Else)
            {
                Consume(TokenType.Else);
                elseStatement = ParseCompoundStatement();
            }

            return new IfStatementNode(condition, thenStatement, elseIfStatements, elseStatement);
        }

        private ASTNode ParseWhileStatement()
        {
            Consume(TokenType.While);
            Consume(TokenType.LParen);
            var condition = ParseExpression();
            Consume(TokenType.RParen);
            var body = ParseCompoundStatement();
            return new WhileStatementNode(condition, body);
        }

        private ASTNode ParseBreakStatement()
        {
            Consume(TokenType.Break);
            return new BreakStatementNode();
        }

        private ASTNode ParseContinueStatement()
        {
            Consume(TokenType.Continue);
            return new ContinueStatementNode();
        }

        private ASTNode ParseStatement()
        {
            var node = _currentToken.Type switch
            {
                TokenType.LBracket => ParseCompoundStatement(),
                TokenType.Identifier => ParseAssignmentStatement(),
                TokenType.Print => ParsePrintStatement(),
                TokenType.Input => ParseInputStatement(),
                TokenType.If => ParseIfStatement(),
                TokenType.While => ParseWhileStatement(),
                TokenType.Break => ParseBreakStatement(),
                TokenType.Continue => ParseContinueStatement(),
                _ => ParseEmptyStatement()
            };

            return node;
        }

        private ASTNode ParseAssignmentStatement()
        {
            var left = ParseVariable();
            var token = _currentToken;
            Consume(TokenType.Assign);
            var right = ParseExpression();
            var node = new AssignmentNode(left, token, right);
            return node;
        }
        private ASTNode ParseEmptyStatement() => new EmptyStatementNode();

        private FunctionDefinitionNode ParseFunctionDefinition()
        {
            // Assuming "function" token is already consumed
            string functionName = _currentToken.Value;
            Consume(TokenType.Identifier); // Function name

            Consume(TokenType.LParen); // Opening parenthesis
            List<ParameterNode> parameters = ParseParameters();
            Consume(TokenType.RParen); // Closing parenthesis

            CompoundStatementNode body = ParseCompoundStatement(); // Function body

            return new FunctionDefinitionNode(functionName, parameters, body);
        }

        private List<ParameterNode> ParseParameters()
        {
            List<ParameterNode> parameters = new List<ParameterNode>();

            if (_currentToken.Type != TokenType.RParen) // Check if parameter list is empty
            {
                do
                {
                    if (_currentToken.Type == TokenType.Identifier)
                    {
                        string paramName = _currentToken.Value.ToString();
                        parameters.Add(new ParameterNode(paramName));
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

        private CompoundStatementNode ParseMainBlock() => ParseCompoundStatement();

        private ASTNode ParseProgram()
        {
            List<FunctionDefinitionNode> functions = new List<FunctionDefinitionNode>();

            while (_currentToken.Type != TokenType.EOF && _currentToken.Type != TokenType.Main)
            {
                if (_currentToken.Type == TokenType.Function)
                {
                    Consume(TokenType.Function);
                    functions.Add(ParseFunctionDefinition());
                }
                else
                    throw new Exception("Unexpected token: " + _currentToken.Type);
            }

            if (_currentToken.Type == TokenType.Main)
            {
                Consume(TokenType.Main);
                var mainBlock = ParseMainBlock();
                return new ProgramNode(functions, mainBlock);
            }
            else
                throw new Exception("Missing main block in the program");
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

        private ASTNode ParseOrExpression()
        {
            var node = ParseAndExpression();

            while (_currentToken.Type == TokenType.Or)
            {
                var token = _currentToken;
                Consume(token.Type); // Consume the Or operator
                node = new BinaryOperatorNode(node, token, ParseAndExpression());
            }

            return node;
        }

        private ASTNode ParseAndExpression()
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