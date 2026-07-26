using CHG.Scripts.Agents;
using Members.CHG.Scripts.Players.FSM;
using UnityEngine;

namespace CHG.Scripts.Players.FSM
{
    public class PlayerRunState : AbstractPlayerState
    {
        public PlayerRunState(Agent agent, int stateClipHash) : base(agent, stateClipHash)
        {
        }

        public override void Enter(float transitionDuration, int layerIndex = 0)
        {
            base.Enter(transitionDuration, layerIndex);
            _player.PlayerInput.OnMovementChange += HandleMovementChange;
            _controlMovement.SetMovementDirection(_player.PlayerInput.CurrentMovement);
        }

        public override void Exit()
        {
            _player.PlayerInput.OnMovementChange -= HandleMovementChange;
            base.Exit();
        }

        private void HandleMovementChange(Vector2 movementKey)
        {
            _controlMovement.SetMovementDirection(movementKey);
            
            if (movementKey.magnitude < INPUT_DEADZONE)
            {
                _player.ChangeState(PlayerState.IDLE, transitionDuration: 0.1f); 
                return;
            }
        }
    }
}