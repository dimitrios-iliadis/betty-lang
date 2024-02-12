﻿namespace BettyLang.Core
{
    public class Lexer
    {
        private readonly string _input;
        private int _position;
        private char _currentChar;
        private readonly System.Text.StringBuilder _stringBuilder = new();

        private static readonly Dictionary<string, Token> _reservedKeywords = new()
        {
            ["func"] = new Token(TokenType.Func),
            ["true"] = new Token(TokenType.True),
            ["false"] = new Token(TokenType.False),
            ["if"] = new Token(TokenType.If),
            ["elif"] = new Token(TokenType.Elif),
            ["else"] = new Token(TokenType.Else),
            ["for"] = new Token(TokenType.For),
            ["while"] = new Token(TokenType.While),
            ["break"] = new Token(TokenType.Break),
            ["continue"] = new Token(TokenType.Continue),
            ["return"] = new Token(TokenType.Return)
        };

        private static readonly Dictionary<char, TokenType> _singleCharOperators = new()
        {
            ['+'] = TokenType.Plus,
            ['-'] = TokenType.Minus,
            ['*'] = TokenType.Star,
            ['/'] = TokenType.Slash,
            ['^'] = TokenType.Caret,
            ['('] = TokenType.LParen,
            [')'] = TokenType.RParen,
            ['{'] = TokenType.LBrace,
            ['}'] = TokenType.RBrace,
            [';'] = TokenType.Semicolon,
            ['!'] = TokenType.Not,
            ['='] = TokenType.Equal,
            ['<'] = TokenType.LessThan,
            ['>'] = TokenType.GreaterThan,
            [','] = TokenType.Comma,
            ['?'] = TokenType.QuestionMark,
            [':'] = TokenType.Colon,
            ['%'] = TokenType.Modulo
        };

        private static readonly Dictionary<string, TokenType> _doubleCharOperators = new()
        {
            ["=="] = TokenType.EqualEqual,
            ["<="] = TokenType.LessThanOrEqual,
            [">="] = TokenType.GreaterThanOrEqual,
            ["!="] = TokenType.NotEqual,
            ["&&"] = TokenType.And,
            ["||"] = TokenType.Or,
            ["++"] = TokenType.Increment,
            ["--"] = TokenType.Decrement,
            ["+="] = TokenType.PlusEqual,
            ["-="] = TokenType.MinusEqual,
            ["*="] = TokenType.StarEqual,
            ["/="] = TokenType.SlashEqual,
            ["^="] = TokenType.CaretEqual,
            ["%="] = TokenType.ModuloEqual
        };

        public Lexer(string input)
        {
            _input = input;
            _position = 0;
            _currentChar = _input.Length > 0 ? _input[_position] : '\0'; // Handle empty input
        }

        private void Advance()
        {
            _position++;
            if (_position > _input.Length - 1)
                _currentChar = '\0';
            else
                _currentChar = _input[_position];
        }

        private void SkipWhitespace()
        {
            while (_currentChar != '\0' && Char.IsWhiteSpace(_currentChar))
                Advance();
        }

        private string ScanStringLiteral()
        {
            _stringBuilder.Clear();

            Advance(); // Skip the opening quote

            while (_currentChar != '"')
            {
                if (_currentChar == '\\') // Check for escape character
                {
                    Advance(); // Skip the escape character
                    switch (_currentChar)
                    {
                        case 'n': _stringBuilder.Append('\n'); break; // Newline
                        case 't': _stringBuilder.Append('\t'); break; // Tab
                        case '"': _stringBuilder.Append('\"'); break; // Double quote
                        case '\'': _stringBuilder.Append('\''); break; // Single quote
                        case '\\': _stringBuilder.Append('\\'); break; // Backslash
                        default:
                            throw new Exception($"Unrecognized escape sequence: \\{_currentChar}");
                    }
                    Advance(); // Move past the character after the escape
                }
                else
                {
                    _stringBuilder.Append(_currentChar);
                    Advance();
                }

                if (_currentChar == '\0')
                    throw new Exception("Unterminated string literal.");
            }

            Advance(); // Skip the closing quote
            return _stringBuilder.ToString();
        }

        private string ScanNumberLiteral(bool hasLeadingDot)
        {
            _stringBuilder.Clear();

            bool dotEncountered = hasLeadingDot;

            if (hasLeadingDot)
            {
                _stringBuilder.Append("0.");
                Advance(); // Move past the dot character
            }

            while (Char.IsDigit(_currentChar) || _currentChar == '.')
            {
                if (_currentChar == '.')
                {
                    if (dotEncountered) // Throw when encountering multiple dots
                        throw new FormatException("Invalid numeric format with multiple dots.");

                    dotEncountered = true;
                }

                _stringBuilder.Append(_currentChar);
                Advance();
            }

            return _stringBuilder.ToString();
        }

        private Token ScanIdentifierOrKeyword()
        {
            _stringBuilder.Clear();

            while (_currentChar != '\0' && Char.IsLetterOrDigit(_currentChar))
            {
                _stringBuilder.Append(_currentChar);
                Advance();
            }

            var result = _stringBuilder.ToString().ToLower();

            if (_reservedKeywords.TryGetValue(result, out Token token))
                return token;

            return new Token(TokenType.Identifier, result);
        }

        private char PeekNextChar() => (_position + 1 >= _input.Length) ? '\0' : _input[_position + 1];

        private void SkipComment()
        {
            while (_currentChar != '\0' && _currentChar != '\n')
                Advance();
        }

        public Token PeekNextToken()
        {
            // Save the current state
            var currentPosition = _position;
            var currentChar = _currentChar;

            var nextToken = GetNextToken();

            // Restore the saved state
            _position = currentPosition;
            _currentChar = currentChar;

            return nextToken;
        }

        public Token GetNextToken()
        {
            while (_currentChar != '\0')
            {
                if (Char.IsWhiteSpace(_currentChar))
                {
                    SkipWhitespace();
                    continue;
                }

                if (_currentChar == '/' && PeekNextChar() == '/')
                {
                    SkipComment();
                    continue;
                }

                if (Char.IsLetter(_currentChar))
                    return ScanIdentifierOrKeyword();

                if (Char.IsDigit(_currentChar))
                    return new Token(TokenType.NumberLiteral, ScanNumberLiteral(hasLeadingDot: false));

                if (_currentChar == '.' && Char.IsDigit(PeekNextChar()))
                    return new Token(TokenType.NumberLiteral, ScanNumberLiteral(hasLeadingDot: true));

                if (_currentChar == '"')
                    return new Token(TokenType.StringLiteral, ScanStringLiteral());

                string doubleCharOperator = _currentChar.ToString() + PeekNextChar();
                if (_doubleCharOperators.TryGetValue(doubleCharOperator, out TokenType tokenType))
                {
                    Advance();
                    Advance();
                    return new Token(tokenType);
                }

                if (_singleCharOperators.TryGetValue(_currentChar, out tokenType))
                {
                    Advance();
                    return new Token(tokenType);
                }

                throw new Exception($"Unrecognized character: {_currentChar}");
            }

            return new Token(TokenType.EOF);
        }
    }
}