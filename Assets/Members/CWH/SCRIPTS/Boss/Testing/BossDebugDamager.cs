using Boss.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Boss.Testing
{
    // 테스트용: R키를 누르고 있던 시간에 비례해 보스에게 데미지를 준다.
    public class BossDebugDamager : MonoBehaviour
    {
        [SerializeField] private GameObject bossTarget;
        [SerializeField] private float minDamage = 10f;
        [SerializeField] private float maxDamage = 500f;
        [SerializeField] private float maxHoldSeconds = 10f;

        private IDamageable health;
        private IHealthResettable healthResettable;
        private float holdTimer;
        private bool isHolding;

        private void Awake()
        {
            if (bossTarget != null)
            {
                health = bossTarget.GetComponent<IDamageable>();
                healthResettable = bossTarget.GetComponent<IHealthResettable>();
            }
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null || health == null)
            {
                return;
            }

            if (keyboard.rKey.wasPressedThisFrame)
            {
                isHolding = true;
                holdTimer = 0f;
            }

            if (isHolding && keyboard.rKey.isPressed)
            {
                holdTimer = Mathf.Min(holdTimer + Time.deltaTime, maxHoldSeconds);
            }

            if (isHolding && keyboard.rKey.wasReleasedThisFrame)
            {
                isHolding = false;

                float t = holdTimer / maxHoldSeconds;
                float damage = Mathf.Lerp(minDamage, maxDamage, t);
                health.TakeDamage(damage);
            }

            if (keyboard.tabKey.wasPressedThisFrame)
            {
                healthResettable?.ResetToFull();
            }
        }
    }
}
