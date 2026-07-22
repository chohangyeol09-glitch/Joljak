using Members.CHG.Scripts.CoreSystem.ModuleSystem;
using UnityEngine;

namespace Members.CHG.Scripts.Players
{
    /// 모델(몸)을 목표 방향으로 회전 속도 제한을 걸어 돌린다.
    /// 평상시엔 이동 방향, 조준 중엔 카메라 방향을 본다.
    /// 루트를 돌리면 카메라가 딸려 돌기 때문에 반드시 모델 자식만 돌린다
    public class PlayerRotationModule : MonoBehaviour, IModule
    {
        [SerializeField] private Transform visual;
        [SerializeField] private float turnSpeed = 720f;   // 초당 도(度)

        private PlayerMovementModule _movement;
        private PlayerLookModule _look;

        /// 조준 중이면 이동과 무관하게 카메라 방향을 본다(게걸음)
        public bool IsAiming { get; set; }

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
            Vector3 target = IsAiming ? AimTarget() : MoveTarget();

            //목표가 없으면(제자리) 현재 방향을 유지한다. 억지로 돌리지 않는다
            if (target.sqrMagnitude < Mathf.Epsilon) return;

            Quaternion goal = Quaternion.LookRotation(target);
            visual.rotation = Quaternion.RotateTowards(
                visual.rotation, goal, turnSpeed * Time.fixedDeltaTime);
        }

        //실제로 움직이는 방향을 본다. 수평만
        private Vector3 MoveTarget()
        {
            Vector3 v = _movement.HorizontalVelocity;
            v.y = 0f;
            return v;
        }

        //몸은 pitch를 따라가지 않으므로 yaw만 사용한다
        private Vector3 AimTarget() => _look.YawRotation * Vector3.forward;
    }
}
