using CHG.Scripts.Agents;
using UnityEngine;

namespace CHG.Scripts.Players.FSM
{
    public class PlayerStunState : AbstractPlayerState
    {
        private readonly ConstraintModule _constraint;
        
        public PlayerStunState(Agent agent, int stateClipHash) : base(agent, stateClipHash)
        {
            _constraint = agent.GetModule<ConstraintModule>();
        }

        public override void Enter(float transitionDuration, int layerIndex = 0)
        {
            base.Enter(transitionDuration, layerIndex);
            _controlMovement.SetMovementDirection(Vector2.zero);
            
            _constraint.Add(ConstraintType.Rooted, this);
            _constraint.Add(ConstraintType.Disarmed, this);
            _constraint.Add(ConstraintType.Silenced, this);
        }

        public override void Exit()
        {
            _constraint.Remove(ConstraintType.Rooted, this);
            _constraint.Remove(ConstraintType.Disarmed, this);
            _constraint.Remove(ConstraintType.Silenced, this);
            base.Exit();
        }
    }
}