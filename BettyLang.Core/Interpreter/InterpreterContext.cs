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
        public InterpreterValue LastReturnValue { get; set; } = InterpreterValue.None();
        public bool IsInLoop { get; set; } = false;

        public void ResetFlowState()
        {
            FlowState = ControlFlowState.Normal;
        }
    }
}