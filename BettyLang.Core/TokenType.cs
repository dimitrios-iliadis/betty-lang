﻿namespace BettyLang.Core
{
    public enum TokenType
    {
        NumberLiteral, StringLiteral, TrueLiteral, FalseLiteral, Identifier,

        Not, And, Or,

        Plus, Minus, Mul, Div, Caret,

        LParen, RParen, LBracket, RBracket, Semicolon, Comma,

        Main, Function,

        Assign, Print, Input, If, Elif, Else, While, Break, Continue, Return,
        
        Equal, LessThan, GreaterThan, LessThanOrEqual, GreaterThanOrEqual, NotEqual,

        EOF
    }
}