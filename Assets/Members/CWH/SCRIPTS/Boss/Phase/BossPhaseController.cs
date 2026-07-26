using System;
using Boss.Core;
using UnityEngine;

namespace Boss.Phase
{
    public class BossPhaseController : MonoBehaviour, IBossPhaseController
    {
        [SerializeField] private BossStatsSO stats;

        private IDamageable health;

        public int CurrentPhase { get; private set; }
        public event Action<int> PhaseChanged;

        private void Awake()
        {
            health = GetComponent<IDamageable>();
            health.HealthChanged += OnHealthChanged;
        }

        private void OnDestroy()
        {
            if (health != null)
            {
                health.HealthChanged -= OnHealthChanged;
            }
        }

        private void OnHealthChanged(float current, float max)
        {
            float ratio = current / max;
            var thresholds = stats.PhaseHealthThresholds;

            int phase = 0;
            for (int i = 0; i < thresholds.Count; i++)
            {
                if (ratio <= thresholds[i])
                {
                    phase = i + 1;
                }
            }

            if (phase != CurrentPhase)
            {
                CurrentPhase = phase;
                PhaseChanged?.Invoke(CurrentPhase);
            }
        }
    }
}
