namespace BettyLang.Core.Interpreter
{
    public enum ValueType
    {
        Number,
        String,
        Boolean,
        Char,
        List,
        None
    }

    public readonly struct Value : IEquatable<Value>
    {
        public readonly ValueType Type;
        private readonly double _number;
        private readonly char _char;
        private readonly int _stringId;
        private readonly bool _boolean;
        private readonly List<Value> _list = []; // Field to hold list data

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

        private Value(char character) : this(ValueType.Char)
        {
            _char = character;
            _number = character + 0;
        }

        private Value(List<Value> list) : this(ValueType.List)
        {
            _list = list;
        }

        public static Value FromString(string str)
        {
            int stringId = StringTable.AddString(str);
            return new Value(stringId);
        }

        public static Value FromNumber(double number) => new(number);
        public static Value FromBoolean(bool boolean) => new(boolean);
        public static Value FromChar(char character) => new(character);

        public static Value FromList(List<Value> list) => new(list);
        public static Value None() => new(ValueType.None);

        // This method creates a new list with the added element
        public static Value AddToList(Value list, Value newItem)
        {
            if (list.Type != ValueType.List)
                throw new InvalidOperationException("The left operand must be a list.");

            // If the new item is a list, concatenate the two lists
            if (newItem.Type == ValueType.List)
                return Value.FromList([.. list.AsList(), .. newItem.AsList()]);

            // Clone the existing list and add the new item
            var newList = new List<Value>(list.AsList()) { newItem };
            return Value.FromList(newList);
        }

        public readonly char AsChar()
        {
            if (Type != ValueType.Char)
                throw new InvalidOperationException($"Expected a character, but got {Type}.");
            return _char;
        }

        public readonly double AsNumber()
        {
            if (Type == ValueType.Char)
                return _number;  // Return the stored numeric value of the character

            if (Type != ValueType.Number)
                throw new InvalidOperationException($"Expected a {ValueType.Number}, but got {Type}.");
            return _number;
        }

        public readonly string AsString()
        {
            if (Type != ValueType.String)
                throw new InvalidOperationException($"Expected a {ValueType.String}, but got {Type}.");
            return StringTable.GetString(_stringId);
        }

        public readonly bool AsBoolean()
        {
            if (Type != ValueType.Boolean)
                throw new InvalidOperationException($"Expected a {ValueType.Boolean}, but got {Type}.");
            return _boolean;
        }

        public List<Value> AsList()
        {
            if (Type == ValueType.String)
                return new List<Value>(StringTable.GetString(_stringId).Select(c => Value.FromChar(c)));

            if (Type != ValueType.List)
                throw new InvalidOperationException($"Expected a {ValueType.List}, but got {Type}.");
            return _list;
        }

        public override string ToString()
        {
            return Type switch
            {
                ValueType.Number => _number.ToString(),
                ValueType.String => StringTable.GetString(_stringId),
                ValueType.Boolean => _boolean.ToString(),
                ValueType.Char => _char.ToString(),
                ValueType.List => $"[{string.Join(", ", _list.Select(item => item.ToString()))}]",
                ValueType.None => "None",
                _ => throw new InvalidOperationException($"Unknown type {Type}.")
            };
        }

        public bool Equals(Value other)
        {
            return Type switch
            {
                ValueType.Number => _number == other._number,
                ValueType.String => _stringId == other._stringId,
                ValueType.Boolean => _boolean == other._boolean,
                ValueType.Char => _char == other._char,
                ValueType.List => _list.SequenceEqual(other._list),
                ValueType.None => true,
                _ => throw new InvalidOperationException($"Unknown type {Type}."),
            };
        }

        // Override Equals to handle object type and call the IEquatable implementation
        public override bool Equals(object? obj)
        {
            if (obj is Value other)
            {
                return Equals(other);
            }
            return false;
        }

        // Override GetHashCode to ensure equality comparisons work correctly
        public override int GetHashCode()
        {
            return Type switch
            {
                ValueType.Number => _number.GetHashCode(),
                ValueType.String => _stringId,
                ValueType.Boolean => _boolean.GetHashCode(),
                ValueType.Char => _char.GetHashCode(),
                ValueType.List => _list.GetHashCode(),
                ValueType.None => 0,
                _ => throw new InvalidOperationException($"Unknown type {Type}."),
            };
        }

        public static bool operator ==(Value left, Value right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Value left, Value right)
        {
            return !(left == right);
        }
    }
}