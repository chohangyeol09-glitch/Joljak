namespace Members.CHG.Scripts.Agents.FSM
{
    public abstract class AgentState
    {
        protected Agent _agent;
        protected readonly int _stateClipHash;

        protected IRenderer _renderer;

        public AgentState(Agent agent, int stateClipHash)
        {
            _agent = agent;
            _stateClipHash = stateClipHash;
            _renderer = _agent.GetModule<IRenderer>();
        }

        public virtual void Enter(float transitionDuration, int layerIndex = 0)
        {
            _renderer.PlayClip(_stateClipHash, 0f, transitionDuration, layerIndex);
        }

        /// 입력 읽기, 상태 전환 판정
        public virtual void Update() { }

        /// 물리에 영향을 주는 로직. 이동 모듈과 같은 틱에서 돈다
        public virtual void FixedUpdate() { }

        public virtual void Exit() {}
    }
}