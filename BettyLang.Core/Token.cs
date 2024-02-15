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

        Func, Global,

        Equal, If, Elif, Else, For, While, Do, Break, Continue, Return,

        EqualEqual, LessThan, GreaterThan, LessThanOrEqual, GreaterThanOrEqual, NotEqual,

        EOF
    }

    public readonly struct Token(TokenType type, object? value = default, int line = 0, int column = 0)
    {
        public TokenType Type { get; } = type;
        public object? Value { get; } = value;
        public int Line { get; } = line;
        public int Column { get; } = column;
    }
}