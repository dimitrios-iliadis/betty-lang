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

        public static Value FromNumber(double number) => new Value(number);
        public static Value FromBoolean(bool boolean) => new Value(boolean);
        public static Value FromChar(char character) => new Value(character);
        public static Value FromList(List<Value> list) => new Value(list);
        public static Value None() => new Value(ValueType.None);

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
                return _char + 0; // Convert the character to its numeric value.

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
            if (Type != other.Type)
                return false;

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

        public override bool Equals(object? obj)
        {
            return obj is Value other && Equals(other);
        }

        public override int GetHashCode()
        {
            // Choose a large prime number to start with and another to combine.
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + Type.GetHashCode();
                switch (Type)
                {
                    case ValueType.Number:
                        hash = hash * 23 + _number.GetHashCode();
                        break;
                    case ValueType.String:
                        hash = hash * 23 + _stringId.GetHashCode();
                        break;
                    case ValueType.Boolean:
                        hash = hash * 23 + _boolean.GetHashCode();
                        break;
                    case ValueType.Char:
                        hash = hash * 23 + _char.GetHashCode();
                        break;
                    case ValueType.List:
                        foreach (var val in _list)
                        {
                            hash = hash * 23 + val.GetHashCode();
                        }
                        break;
                    case ValueType.None:
                        // Nothing needed for None type
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown type {Type}.");
                }
                return hash;
            }
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