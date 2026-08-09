using System;
using CHG.Scripts.Agents;
using CHG.Scripts.SkillSystem.SkillDatas;
using UnityEngine;

namespace CHG.Scripts.SkillSystem
{
    public abstract class AbstractSkill : MonoBehaviour, ISkill
    {
        [field: SerializeField] public SkillDataSO Data { get; private set; }

        [Tooltip("이 스킬이 거는 제약")]
        [SerializeField] private ConstraintType[] castConstraints;
        
        public bool IsUsing { get; private set; }

        public event Action<float> OnCooldownStarted;
        public event Action OnUsableChanged;
        public event Action OnSkillStarted;
        public event Action<ISkill> OnSkillEnded;

        public float RemainingCooldown => Mathf.Max(0f, _cooldownEndTime - Time.time);
        public float NormalizedCooldown => Data.Cooldown > 0f ? Mathf.Clamp01(RemainingCooldown / Data.Cooldown) : 0f;
        public bool IsOnCooldown => RemainingCooldown > 0f;
        public virtual bool IsBlocked => _constraint != null && _constraint.Has(ConstraintType.Silenced);
        
        protected ISkillModule _module;
        protected ConstraintModule _constraint;
        
        private float _cooldownEndTime = float.NegativeInfinity;
        
        public virtual void InitializeSkill(ISkillModule module, SkillDataSO data)
        {
            _module = module;
            if (data != null) Data = data;
            _constraint = module.Owner.GetModule<ConstraintModule>();
            
            Debug.Assert(Data != null, $"SkillData is null : {gameObject.name}");
            
            if (_constraint == null) return;

            _constraint.OnConstraintAdded += HandleConstraintChanged;
            _constraint.OnConstraintRemoved += HandleConstraintChanged;
        }

        protected virtual void OnDestroy()
        {
            if (_constraint == null) return;
            
            _constraint.OnConstraintAdded -= HandleConstraintChanged;
            _constraint.OnConstraintRemoved -= HandleConstraintChanged;

            if (IsUsing) ReleaseCastConstraints();
        }

        private void HandleConstraintChanged(ConstraintType type)
        {
            if (type == ConstraintType.Silenced)
                OnUsableChanged?.Invoke();
        }
        
        public virtual bool CanUseSkill(GameObject target = null)
        {
            if (IsUsing || IsOnCooldown) return false;
            if (_constraint != null && _constraint.Has(ConstraintType.Silenced)) return false;
            
            return true;
        }

        public void UseSkill(GameObject target = null)
        {
            if (!CanUseSkill(target)) return;
            
            IsUsing = true;
            ApplyCastConstraints();
            
            OnSkillStarted?.Invoke();
            Execute(target);
        }

        public virtual void ReleaseSkill(){}
        protected abstract void Execute(GameObject target);

        public void StopSkill()
        {
            if (!IsUsing) return;

            Cancel();
            EndSkill();
        }
        
        protected virtual void Cancel() {}

        protected void EndSkill()
        {
            if (!IsUsing) return;

            IsUsing = false;
            ReleaseCastConstraints();
            OnSkillEnded?.Invoke(this);
        }

        protected void StartCooldown()
        {
            _cooldownEndTime = Time.time + Data.Cooldown;
            OnCooldownStarted?.Invoke(Data.Cooldown);
        }

        private void ApplyCastConstraints()
        {
            if (_constraint == null) return;
            foreach (ConstraintType type in castConstraints)
                _constraint.Add(type,this);
        }
        
        private void ReleaseCastConstraints()
        {
            if (_constraint == null) return;
            foreach (ConstraintType type in castConstraints)
                _constraint.Remove(type, this);
        }
        
        protected void RaiseUsableChanged() => OnUsableChanged?.Invoke();
    }
}