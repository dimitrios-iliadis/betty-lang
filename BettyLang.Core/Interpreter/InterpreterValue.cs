﻿using System.Runtime.InteropServices;

namespace BettyLang.Core.Interpreter
{
    public enum ValueType
    {
        Number,
        String,
        Boolean,
        None
    }

    [StructLayout(LayoutKind.Explicit)]
    public readonly struct InterpreterValue
    {
        [FieldOffset(0)]
        public readonly ValueType Type;

        [FieldOffset(4)]
        private readonly double _number;

        [FieldOffset(4)]
        private readonly int _stringId;

        [FieldOffset(4)]
        private readonly bool _boolean;

        private InterpreterValue(ValueType type) : this()
        {
            Type = type;
        }

        private InterpreterValue(double number) : this(ValueType.Number)
        {
            _number = number;
        }

        private InterpreterValue(int stringId) : this(ValueType.String)
        {
            _stringId = stringId;
        }

        private InterpreterValue(bool boolean) : this(ValueType.Boolean)
        {
            _boolean = boolean;
        }

        public static InterpreterValue FromString(string str)
        {
            int stringId = StringTable.AddString(str);
            return new InterpreterValue(stringId);
        }

        public static InterpreterValue FromNumber(double number) => new InterpreterValue(number);
        public static InterpreterValue FromBoolean(bool boolean) => new InterpreterValue(boolean);
        public static InterpreterValue None() => new InterpreterValue(ValueType.None);

        public readonly double AsNumber()
        {
            if (Type != ValueType.Number)
                throw new InvalidOperationException($"Expected a number, but got {Type}.");
            return _number;
        }

        public readonly string AsString()
        {
            if (Type != ValueType.String)
                throw new InvalidOperationException($"Expected a string, but got {Type}.");
            return StringTable.GetString(_stringId);
        }

        public readonly bool AsBoolean()
        {
            if (Type != ValueType.Boolean)
                throw new InvalidOperationException($"Expected a boolean, but got {Type}.");
            return _boolean;
        }
    }
}