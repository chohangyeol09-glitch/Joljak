using Boss.Core;
using UnityEngine;

namespace Boss.Animation
{
    [RequireComponent(typeof(Animator))]
    public class BossAnimationSync : MonoBehaviour, IBossAnimator
    {
        [SerializeField] private string speedParameter = "Speed";
        [SerializeField] private string deathTrigger = "Die";
        [SerializeField] private float turnSpeedInfluence = 0.02f;
        [SerializeField] private float speedDampTime = 0.08f;
        [SerializeField] private float moveThreshold = 0.1f;
        [SerializeField] private float minimumMoveHoldTime = 0.3f;

        private Animator animator;
        private IMovable mover;
        private bool isDead;
        private float previousYaw;
        private float moveHoldTimer;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            mover = GetComponent<IMovable>();
            previousYaw = transform.eulerAngles.y;
        }

        private void Update()
        {
            float currentYaw = transform.eulerAngles.y;
            float angularSpeed = Mathf.Abs(Mathf.DeltaAngle(previousYaw, currentYaw)) / Time.deltaTime;
            previousYaw = currentYaw;

            if (isDead)
            {
                return;
            }

            float linearSpeed = mover.CurrentSpeed;
            float turnSpeed = angularSpeed * turnSpeedInfluence;

            float rawSpeed;
            if (linearSpeed > moveThreshold)
            {
                // 실제로 이동 중일 땐 NavMeshAgent 자체 감속 곡선을 그대로 따른다 (홀드/댐핑 추가 없음 - 이중 감속으로 미끄러지는 것 방지).
                rawSpeed = linearSpeed;
                moveHoldTimer = 0f;
            }
            else if (turnSpeed > moveThreshold)
            {
                // 이동 없이 제자리 회전만 할 땐 너무 짧게 반짝이지 않도록 최소 유지시간을 둔다.
                moveHoldTimer = minimumMoveHoldTime;
                rawSpeed = turnSpeed;
            }
            else if (moveHoldTimer > 0f)
            {
                moveHoldTimer -= Time.deltaTime;
                rawSpeed = moveThreshold;
            }
            else
            {
                rawSpeed = 0f;
            }

            animator.SetFloat(speedParameter, rawSpeed, speedDampTime, Time.deltaTime);
        }

        public void PlayAttack(string attackId)
        {
            if (isDead)
            {
                return;
            }

            animator.SetTrigger(attackId);
        }

        public void PlayDeath()
        {
            isDead = true;
            animator.SetTrigger(deathTrigger);
        }
    }
}
