namespace BettyLang.Core
{
    public enum TokenType
    {
        NumberLiteral, StringLiteral, TrueLiteral, FalseLiteral, Identifier,

        Not, And, Or,

        Plus, Minus, Star, Slash, Caret, Modulo,

        Increment, Decrement,

        LParen, RParen, LBrace, RBrace, Semicolon, Comma, QuestionMark, Colon,

        Func,

        Equal, If, Elif, Else, While, Break, Continue, Return,

        EqualEqual, LessThan, GreaterThan, LessThanOrEqual, GreaterThanOrEqual, NotEqual,

        EOF
    }

    public readonly struct Token(TokenType type, string? value = default)
    {
        public TokenType Type { get; } = type;
        public string? Value { get; } = value;
    }
}