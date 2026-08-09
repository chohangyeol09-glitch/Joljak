using System;
using UnityEngine;

namespace CHG.Scripts.SkillSystem
{
    public abstract class ChargeSkill : AbstractSkill
    {
        [SerializeField] private float maxChargeTime = 1.5f;
        [Tooltip("최대 차징 후 유예시간")]
        [SerializeField] private float graceTime = 0.5f;
        
        protected float _chargeStartTime;
        private GameObject _target;
        
        protected virtual float MaxChargeTime => maxChargeTime;
        protected virtual float GraceTime => graceTime;

        public float ChargeRatio => IsUsing ? Mathf.Clamp01((Time.time - _chargeStartTime) / 
                                                Mathf.Max(0.0001f, MaxChargeTime)) : 0f;

        public bool IsFullyCharged => IsUsing && Time.time - _chargeStartTime >= MaxChargeTime;
        
        protected sealed override void Execute(GameObject target)
        {
            _chargeStartTime = Time.time;
            _target = target;
            
            OnChargeStart(target);
        }

        protected virtual void Update()
        {
            if (!IsUsing) return;

            if (Time.time - _chargeStartTime >= MaxChargeTime + GraceTime)
            {
                StopSkill();
                StartCooldown();
            }
            
        }

        public sealed override void ReleaseSkill()
        {
            if (!IsUsing) return;

            Fire(_target, ChargeRatio);
            StartCooldown();
            EndSkill();
        }

        protected override void Cancel()
        {
            OnChargeCancel();
        }
        
        protected virtual void OnChargeStart(GameObject target) {}
        protected virtual void OnChargeCancel() {}
        protected abstract void Fire(GameObject target, float chargeRatio);
    }
}