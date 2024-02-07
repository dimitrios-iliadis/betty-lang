using System.Runtime.InteropServices;

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
    public readonly struct Value
    {
        [FieldOffset(0)]
        public readonly ValueType Type;

        [FieldOffset(4)]
        private readonly double _number;

        [FieldOffset(4)]
        private readonly int _stringId;

        [FieldOffset(4)]
        private readonly bool _boolean;

        private Value(ValueType type) : this()
        {
            Type = type;
        }

        private Value(double number) : this(ValueType.Number)
        {
            _number = number;
        }

        private Value(int stringId) : this(ValueType.String)
        {
            _stringId = stringId;
        }

        private Value(bool boolean) : this(ValueType.Boolean)
        {
            _boolean = boolean;
        }

        public static Value FromString(string str)
        {
            int stringId = StringTable.AddString(str);
            return new Value(stringId);
        }

        public static Value FromNumber(double number) => new Value(number);
        public static Value FromBoolean(bool boolean) => new Value(boolean);
        public static Value None() => new Value(ValueType.None);

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
