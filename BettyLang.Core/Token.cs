namespace BettyLang.Core
{
    public enum TokenType
    {
        NumberLiteral, CharLiteral, StringLiteral, True, False, Identifier,

        Not, And, Or,

        Plus, Minus, Star, Slash, Caret, Modulo,

        Increment, Decrement, PlusEqual, MinusEqual, StarEqual, SlashEqual, CaretEqual, ModuloEqual,

        LParen, RParen, LBrace, RBrace, Semicolon, Comma, QuestionMark, Colon,

        Func,

        Equal, If, Elif, Else, For, While, Do, Break, Continue, Return,

        EqualEqual, LessThan, GreaterThan, LessThanOrEqual, GreaterThanOrEqual, NotEqual,

        EOF
    }

    public readonly struct Token(TokenType type, string? value = default)
    {
        public TokenType Type { get; } = type;
        public string? Value { get; } = value;
    }
}