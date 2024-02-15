namespace BettyLang.Core.Interpreter
{
    public enum ResultType
    {
        Number,
        String,
        Boolean,
        Char,
        List,
        None
    }

    public readonly struct InterpreterResult : IEquatable<InterpreterResult>
    {
        public readonly ResultType Type;
        private readonly double _number;
        private readonly char _char;
        private readonly int _stringId;
        private readonly bool _boolean;
        private readonly List<InterpreterResult> _list = []; // Field to hold list data

        private InterpreterResult(ResultType type) : this()
        {
            Type = type;
        }

        private InterpreterResult(double number) : this(ResultType.Number)
        {
            _number = number;
        }

        private InterpreterResult(int stringId) : this(ResultType.String)
        {
            _stringId = stringId;
        }

        private InterpreterResult(bool boolean) : this(ResultType.Boolean)
        {
            _boolean = boolean;
        }

        private InterpreterResult(char character) : this(ResultType.Char)
        {
            _char = character;
        }

        private InterpreterResult(List<InterpreterResult> list) : this(ResultType.List)
        {
            _list = list;
        }

        public static InterpreterResult FromString(string str)
        {
            int stringId = StringTable.AddString(str);
            return new InterpreterResult(stringId);
        }

        public static InterpreterResult FromNumber(double number) => new InterpreterResult(number);
        public static InterpreterResult FromBoolean(bool boolean) => new InterpreterResult(boolean);
        public static InterpreterResult FromChar(char character) => new InterpreterResult(character);
        public static InterpreterResult FromList(List<InterpreterResult> list) => new InterpreterResult(list);
        public static InterpreterResult None() => new InterpreterResult(ResultType.None);

        // This method creates a new list with the added element
        public static InterpreterResult AddToList(InterpreterResult list, InterpreterResult newItem)
        {
            if (list.Type != ResultType.List)
                throw new InvalidOperationException("The left operand must be a list.");

            // If the new item is a list, concatenate the two lists
            if (newItem.Type == ResultType.List)
                return InterpreterResult.FromList([.. list.AsList(), .. newItem.AsList()]);

            // Clone the existing list and add the new item
            var newList = new List<InterpreterResult>(list.AsList()) { newItem };
            return InterpreterResult.FromList(newList);
        }

        public readonly char AsChar()
        {
            if (Type != ResultType.Char)
                throw new InvalidOperationException($"Expected a character, but got {Type}.");
            return _char;
        }

        public readonly double AsNumber()
        {
            if (Type == ResultType.Char)
                return _char + 0; // Convert the character to its numeric value.

            if (Type != ResultType.Number)
                throw new InvalidOperationException($"Expected a {ResultType.Number}, but got {Type}.");
            return _number;
        }

        public readonly string AsString()
        {
            if (Type != ResultType.String)
                throw new InvalidOperationException($"Expected a {ResultType.String}, but got {Type}.");
            return StringTable.GetString(_stringId);
        }

        public readonly bool AsBoolean()
        {
            if (Type != ResultType.Boolean)
                throw new InvalidOperationException($"Expected a {ResultType.Boolean}, but got {Type}.");
            return _boolean;
        }

        public List<InterpreterResult> AsList()
        {
            if (Type != ResultType.List)
                throw new InvalidOperationException($"Expected a {ResultType.List}, but got {Type}.");
            return _list;
        }

        public override string ToString()
        {
            return Type switch
            {
                ResultType.Number => _number.ToString(),
                ResultType.String => StringTable.GetString(_stringId),
                ResultType.Boolean => _boolean.ToString(),
                ResultType.Char => _char.ToString(),
                ResultType.List => $"[{string.Join(", ", _list.Select(item => item.ToString()))}]",
                ResultType.None => "None",
                _ => throw new InvalidOperationException($"Unknown type {Type}.")
            };
        }

        public bool Equals(InterpreterResult other)
        {
            if (Type != other.Type)
                return false;

            return Type switch
            {
                ResultType.Number => _number == other._number,
                ResultType.String => _stringId == other._stringId,
                ResultType.Boolean => _boolean == other._boolean,
                ResultType.Char => _char == other._char,
                ResultType.List => _list.SequenceEqual(other._list),
                ResultType.None => true,
                _ => throw new InvalidOperationException($"Unknown type {Type}."),
            };
        }

        public override bool Equals(object? obj)
        {
            return obj is InterpreterResult other && Equals(other);
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
                    case ResultType.Number:
                        hash = hash * 23 + _number.GetHashCode();
                        break;
                    case ResultType.String:
                        hash = hash * 23 + _stringId.GetHashCode();
                        break;
                    case ResultType.Boolean:
                        hash = hash * 23 + _boolean.GetHashCode();
                        break;
                    case ResultType.Char:
                        hash = hash * 23 + _char.GetHashCode();
                        break;
                    case ResultType.List:
                        foreach (var val in _list)
                        {
                            hash = hash * 23 + val.GetHashCode();
                        }
                        break;
                    case ResultType.None:
                        // Nothing needed for None type
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown type {Type}.");
                }
                return hash;
            }
        }

        public static bool operator ==(InterpreterResult left, InterpreterResult right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(InterpreterResult left, InterpreterResult right)
        {
            return !(left == right);
        }
    }
}