using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG.Scripts.Players
{
    public class PlayerRotationModule : MonoBehaviour, IModule
    {
        [SerializeField] private Transform visual;
        [SerializeField] private Transform chest;
        [SerializeField] private float turnSpeed = 720f;
        [SerializeField] private float maxChestTween;

        private PlayerMovementModule _movement;
        private PlayerLookModule _look;
        

        public Vector3 Forward => visual.forward;
        
        public void Initialize(ModuleOwner owner)
        {
            Debug.Assert(visual != null, $"Model is null : {gameObject.name}");

            _movement = owner.GetModule<PlayerMovementModule>();
            _look = owner.GetModule<PlayerLookModule>();

            Debug.Assert(_movement != null, $"MovementModule is null : {gameObject.name}");
            Debug.Assert(_look != null, $"LookModule is null : {gameObject.name}");
        }

        private void FixedUpdate()
        {
            Vector3 target;
            if (MoveTarget().sqrMagnitude > Mathf.Epsilon)
                target = MoveTarget();
            else if (_look.IsAiming)
                target = AimTarget();
            else return;

            Quaternion goal = Quaternion.LookRotation(target);
            visual.rotation = Quaternion.RotateTowards(visual.rotation, goal, turnSpeed * Time.fixedDeltaTime);
            
        }

        private void LateUpdate()
        {
            if (!_look.IsAiming || chest == null) return;

            float aimYaw = _look.YawRotation.eulerAngles.y;
            float bodyYaw = visual.eulerAngles.y;
            float twist = Mathf.Clamp(Mathf.DeltaAngle(bodyYaw, aimYaw), -maxChestTween, maxChestTween);
            
            chest.Rotate(Vector3.up, twist, Space.World);
        }

        private Vector3 MoveTarget()
        {
            Vector3 v = _movement.HorizontalVelocity;
            v.y = 0f;
            return v;
        }

        private Vector3 AimTarget() => _look.YawRotation * Vector3.forward;

        public void SnapTo(Vector3 direction)
        {
            direction.y = 0f;
            if (direction.sqrMagnitude < Mathf.Epsilon) return;
            visual.rotation = Quaternion.LookRotation(direction.normalized);
        }
    }
}
