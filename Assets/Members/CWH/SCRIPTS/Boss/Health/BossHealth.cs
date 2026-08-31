using System;
using Boss.Core;
using UnityEngine;

namespace Boss.Health
{
    public class BossHealth : MonoBehaviour, IDamageable, IHealthResettable, IHealable
    {
        [SerializeField] private float maxHealth = 500f;

        public float MaxHealth => maxHealth;
        public float CurrentHealth { get; private set; }
        public bool IsDead => CurrentHealth <= 0f;

        public event Action<float, float> HealthChanged;
        public event Action Died;

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        public void TakeDamage(float amount)
        {
            if (IsDead || amount <= 0f)
            {
                return;
            }

            CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
            HealthChanged?.Invoke(CurrentHealth, maxHealth);

            if (IsDead)
            {
                Died?.Invoke();
            }
        }

        public void ResetToFull()
        {
            if (CurrentHealth >= maxHealth)
            {
                return;
            }

            CurrentHealth = maxHealth;
            HealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        public void Heal(float amount)
        {
            if (IsDead || amount <= 0f || CurrentHealth >= maxHealth)
            {
                return;
            }

            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
            HealthChanged?.Invoke(CurrentHealth, maxHealth);
        }
    }
}
