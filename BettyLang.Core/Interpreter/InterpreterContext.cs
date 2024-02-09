namespace BettyLang.Core.Interpreter
{
    public enum ControlFlow
    {
        Normal,
        Return,
        Break,
        Continue
    }

    public class InterpreterContext
    {
        public ControlFlow Flow { get; set; } = ControlFlow.Normal;
        public Value LastReturnValue { get; set; } = Value.None();

        public void ResetFlow()
        {
            Flow = ControlFlow.Normal;
        }
    }
}