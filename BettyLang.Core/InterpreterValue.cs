using System.Runtime.InteropServices;

namespace BettyLang.Core
{
    public enum InterpreterValueType
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
        public readonly InterpreterValueType Type;

        [FieldOffset(4)]
        private readonly double _number;

        [FieldOffset(4)]
        private readonly int _stringId;

        [FieldOffset(4)]
        private readonly bool _boolean;

        private InterpreterValue(InterpreterValueType type) : this()
        {
            Type = type;
        }

        private InterpreterValue(double number) : this(InterpreterValueType.Number)
        {
            _number = number;
        }

        private InterpreterValue(int stringId) : this(InterpreterValueType.String)
        {
            _stringId = stringId;
        }

        private InterpreterValue(bool boolean) : this(InterpreterValueType.Boolean)
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
        public static InterpreterValue None() => new InterpreterValue(InterpreterValueType.None);

        public readonly double AsNumber()
        {
            if (Type != InterpreterValueType.Number)
                throw new InvalidOperationException($"Expected a number, but got {Type}.");
            return _number;
        }

        public readonly string AsString()
        {
            if (Type != InterpreterValueType.String)
                throw new InvalidOperationException($"Expected a string, but got {Type}.");
            return StringTable.GetString(_stringId);
        }

        public readonly bool AsBoolean()
        {
            if (Type != InterpreterValueType.Boolean)
                throw new InvalidOperationException($"Expected a boolean, but got {Type}.");
            return _boolean;
        }
    }
}
