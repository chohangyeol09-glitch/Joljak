using CHG.Scripts.Agents;
using Members.CHG.Scripts.Players.FSM;
using UnityEngine;

namespace CHG.Scripts.Players.FSM
{
    public class PlayerIdleState : AbstractPlayerState
    {
        public PlayerIdleState(Agent agent, int stateClipHash) : base(agent, stateClipHash)
        {
        }

        public override void Enter(float transitionDuration, int layerIndex = 0)
        {
            base.Enter(transitionDuration, layerIndex);
            _controlMovement.SetMovementDirection(Vector2.zero);
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void Update()
        {
            base.Update();
            if (!_controlMovement.CanManualMove) return;
            if (_player.PlayerInput.CurrentMovement.magnitude <= INPUT_DEADZONE) return;
            
            _player.ChangeState(PlayerState.RUN, transitionDuration: 0.1f);
        }
    }
}