namespace BettyLang.Core.Interpreter
{
    public class ScopeManager
    {
        private readonly Stack<Dictionary<string, Value>> _scopes = new();

        public ScopeManager()
        {
        }

        public void EnterScope()
        {
            _scopes.Push([]);
        }

        public void ExitScope()
        {
            _scopes.Pop();
        }

        public void SetVariable(string name, Value value)
        {
            var currentScope = _scopes.Peek();
            currentScope[name] = value;
        }

        public Value LookupVariable(string name)
        {
            foreach (var scope in _scopes)
            {
                if (scope.TryGetValue(name, out var value))
                {
                    return value;
                }
            }
            throw new Exception($"Variable '{name}' not found.");
        }
    }
}