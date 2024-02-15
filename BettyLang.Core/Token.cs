namespace BettyLang.Core
{
    public enum TokenType
    {
        NumberLiteral, CharLiteral, StringLiteral, BooleanLiteral, Identifier,

        Not, And, Or,

        Plus, Minus, Mul, Div, IntDiv, Caret, Mod,

        Increment, Decrement, PlusEqual, MinusEqual, 
        MulEqual, DivEqual, CaretEqual, ModEqual, IntDivEqual,

        LParen, RParen, LBrace, RBrace, LBracket, RBracket, 
        Semicolon, Comma, QuestionMark, Colon,

        Func,

        Equal, If, Elif, Else, For, While, Do, Break, Continue, Return,

        EqualEqual, LessThan, GreaterThan, LessThanOrEqual, GreaterThanOrEqual, NotEqual,

        EOF
    }

    public readonly struct Token(TokenType type, object? value = default)
    {
        public TokenType Type { get; } = type;
        public object? Value { get; } = value;
    }
}