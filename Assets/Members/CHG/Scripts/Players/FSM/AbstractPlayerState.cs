using Members.CHG.Scripts.Agents;
using Members.CHG.Scripts.Agents.FSM;

namespace Members.CHG.Scripts.Players.FSM
{
    public abstract class AbstractPlayerState : AgentState
    {
        protected readonly PlayerController _player;
        protected readonly IControlMovement _controlMovement;
        protected const float INPUT_DEADZONE = 0.1f;
        
        protected AbstractPlayerState(Agent agent, int stateClipHash) : base(agent, stateClipHash)
        {
            _player = agent as PlayerController;
            _controlMovement = agent.GetModule<IControlMovement>();
        }
        
        
    }
}