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
            _controlMovement.SetMovementDirection(_player.PlayerInput.CurrentMovement);
        }


        public override void Update()
        {
            Vector2 input = _player.PlayerInput.CurrentMovement;
            _controlMovement.SetMovementDirection(input);

            if (!_controlMovement.CanManualMove) return;
            
            if (input.magnitude < INPUT_DEADZONE)
                _player.ChangeState(PlayerState.IDLE, transitionDuration: 0.1f);
            
        }
        
        
    }
}