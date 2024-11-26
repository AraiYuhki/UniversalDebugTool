namespace Xeon.UniversalDebugTool.Statemachine
{
    public class WaitState : IState
    {
        private DebugInputSystemActions input;
        public void SetInput(DebugInputSystemActions input) => this.input = input;
        public void OnEnter()
        {
            input.Disable();
        }

        public void OnExit()
        {
        }

        public void Update()
        {
        }

        public void InputUpdate()
        {
        }
    }
}
