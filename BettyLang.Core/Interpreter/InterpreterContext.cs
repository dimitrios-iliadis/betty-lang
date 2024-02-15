namespace BettyLang.Core.Interpreter
{
    public enum ControlFlowState
    {
        Normal,
        Return,
        Break,
        Continue
    }

    public class InterpreterContext
    {
        public ControlFlowState FlowState { get; set; } = ControlFlowState.Normal;
        public InterpreterResult LastReturnValue { get; set; } = InterpreterResult.None();
        public bool IsInLoop { get; set; } = false;
    }
}