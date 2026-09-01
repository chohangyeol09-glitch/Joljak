using System;
using Boss.Core;
using UnityEngine;

namespace Boss.Phase
{
    public class BossPhaseController : MonoBehaviour, IBossPhaseController
    {
        [SerializeField] private BossStatsSO stats;

        private IDamageable _health;
        private IHealCapSettable _healCapSettable;

        public int CurrentPhase { get; private set; }
        public event Action<int> PhaseChanged;

        private void Awake()
        {
            _health = GetComponent<IDamageable>();
            _healCapSettable = GetComponent<IHealCapSettable>();
            _health.HealthChanged += OnHealthChanged;
        }

        private void OnDestroy()
        {
            if (_health != null)
            {
                _health.HealthChanged -= OnHealthChanged;
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
            
            float capRatio = phase == 0 ? 1f : thresholds[phase - 1];
            _healCapSettable?.SetHealCapRatio(capRatio);
            
            if (phase != CurrentPhase)
            {
                CurrentPhase = phase;
                PhaseChanged?.Invoke(CurrentPhase);
            }
        }
    }
}
