namespace BettyLang.Core
{
    public enum TokenType
    {
        NumberLiteral, StringLiteral, BooleanLiteral, Identifier,

        Not, And, Or,

        Plus, Minus, Star, Slash, Caret, Modulo,

        LParen, RParen, LBrace, RBrace, Semicolon, Comma, QuestionMark, Colon,

        Func,

        Equal, If, Elif, Else, While, Break, Continue, Return,

        EqualEqual, LessThan, GreaterThan, LessThanOrEqual, GreaterThanOrEqual, NotEqual,

        EOF
    }

    public readonly struct Token(TokenType type, string value)
    {
        public TokenType Type { get; } = type;
        public string Value { get; } = value;
    }
}