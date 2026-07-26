using CHG.Scripts.Agents;
using Members.CHG.Scripts.Players.FSM;
using UnityEngine;

namespace CHG.Scripts.Players.FSM
{
    public class PlayerDashState : AbstractPlayerState
    {
        private readonly ConstraintModule _constraint;
        private readonly IAimModule _aim;

        private Vector3 _dashDirection;
        private float _endTime;
        private float _lastEndTime = float.NegativeInfinity;
        
        public PlayerDashState(Agent agent, int stateClipHash) : base(agent, stateClipHash)
        {
            _constraint = agent.GetModule<ConstraintModule>();
            _aim = agent.GetModule<IAimModule>();
        }

        public override void Enter(float transitionDuration, int layerIndex = 0)
        {
            base.Enter(transitionDuration, layerIndex);

            _dashDirection = CaptureDirection();
            _endTime = Time.time + _controlMovement.DashDuration;
            
            _constraint.Add(ConstraintType.Rooted, this);
            _constraint.Add(ConstraintType.Disarmed, this);
            _constraint.Add(ConstraintType.Weightless, this);
        }

        public override bool CanEnter() => Time.time >= _lastEndTime + _controlMovement.DashCooldown;

        public override void Exit()
        {
            _constraint.Remove(ConstraintType.Rooted, this);
            _constraint.Remove(ConstraintType.Disarmed, this);
            _constraint.Remove(ConstraintType.Weightless, this);

            _lastEndTime = Time.time;
            base.Exit();
        }

        public override void FixedUpdate()
        {
            if (Time.time >= _endTime)
            {
                Finish();
                return;
            }
            
            _controlMovement.SetDrivenVelocity(_dashDirection * _controlMovement.DashSpeed);
        }

        private Vector3 CaptureDirection()
        {
            Vector3 dir = _controlMovement.HorizontalVelocity;
            dir.y = 0f;

            if (dir.sqrMagnitude < 0.01f)
            {
                dir = _aim.AimForward;
                dir.y = 0f;
            }

            return dir.normalized;
        }
        
        private void Finish()
        {
            bool hasInput = _player.PlayerInput.CurrentMovement.magnitude > INPUT_DEADZONE;
            _player.ChangeState(hasInput ? PlayerState.RUN : PlayerState.IDLE, 0.1f);
        }
    }
}