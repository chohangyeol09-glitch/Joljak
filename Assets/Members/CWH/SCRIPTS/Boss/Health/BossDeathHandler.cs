using Boss.Core;
using UnityEngine;

namespace Boss.Health
{
    public class BossDeathHandler : MonoBehaviour
    {
        private IDamageable health;
        private IMovable mover;
        private IBossAnimator animator;

        private void Awake()
        {
            health = GetComponent<IDamageable>();
            mover = GetComponent<IMovable>();
            animator = GetComponent<IBossAnimator>();
            health.Died += OnDied;
        }

        private void OnDestroy()
        {
            if (health != null)
            {
                health.Died -= OnDied;
            }
        }

        private void OnDied()
        {
            mover?.Stop();
            animator?.PlayDeath();
        }
    }
}
