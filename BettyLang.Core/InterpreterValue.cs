namespace BettyLang.Core
{
    public enum InterpreterValueType
    {
        Number,
        String,
        Boolean,
        None
    }

    public readonly struct InterpreterValue
    {
        public InterpreterValueType Type { get; }

        private readonly double _number;
        private readonly string _string;
        private readonly bool _boolean;

        // Private constructor used for setting the Type directly
        private InterpreterValue(InterpreterValueType type)
        {
            Type = type;
            _number = default;
            _string = string.Empty;
            _boolean = default;
        }

        // Constructor for each type
        private InterpreterValue(double number) : this(InterpreterValueType.Number)
        {
            _number = number;
        }

        private InterpreterValue(string str) : this(InterpreterValueType.String)
        {
            _string = str;
        }

        private InterpreterValue(bool boolean) : this(InterpreterValueType.Boolean)
        {
            _boolean = boolean;
        }

        // Factory methods
        public static InterpreterValue FromNumber(double number) => new InterpreterValue(number);
        public static InterpreterValue FromString(string str) => new InterpreterValue(str);
        public static InterpreterValue FromBoolean(bool boolean) => new InterpreterValue(boolean);
        public static InterpreterValue None() => new InterpreterValue(InterpreterValueType.None);

        // Methods to get the value
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
            return _string;
        }

        public readonly bool AsBoolean()
        {
            if (Type != InterpreterValueType.Boolean)
                throw new InvalidOperationException($"Expected a boolean, but got {Type}.");
            return _boolean;
        }

    }
}