namespace BettyLang.Core.Interpreter
{
    public class ScopeManager
    {
        private readonly Stack<Dictionary<string, InterpreterValue>> _scopes = new();

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

        public void SetVariable(string name, InterpreterValue value)
        {
            var currentScope = _scopes.Peek();
            currentScope[name] = value;
        }

        public InterpreterValue LookupVariable(string name)
        {
            foreach (var scope in _scopes)
            {
                if (scope.TryGetValue(name, out var value))
                {
                    return value;
                }
            }
            throw new Exception($"Variable '{name}' is not defined in any scope.");
        }
    }
}