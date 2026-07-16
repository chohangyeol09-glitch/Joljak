namespace Members.CHG._02.Scripts.Agents.FSM
{
    public abstract class AgentState
    {
        protected Agent _agent;

        public AgentState(Agent agent)
        {
            _agent = agent;
        }

        public virtual void Enter(float transitionDuration) {}

        public virtual void Update() { }

        public virtual void Exit() {}
    }
}