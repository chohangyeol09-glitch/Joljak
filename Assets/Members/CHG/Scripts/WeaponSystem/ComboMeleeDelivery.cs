using System;
using System.Collections;
using CHG.Scripts.Agents;
using CHG.Scripts.CombatSystem;
using CHG.Scripts.CoreSystem.AnimationSystem;
using CHG.Scripts.Players;
using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG.Scripts.WeaponSystem
{
    public class ComboMeleeDelivery : WeaponDelivery
    {
        [Serializable]
        public class ComboStep
        {
            public AnimParamSO Clip;
            public AnimationCurve Curve;
            public float Duration;
            public float DamageMultiplier = 1f;
        }

        [SerializeField] private float comboWindow = 0.8f;
        [SerializeField] private ComboStep[] comboSteps;
        
        private AbstractDamageCaster _damageCaster;
        private IAimModule _aim;
        private IControlMovement _movement;
        private IRenderer _renderer;
        private ConstraintModule _constraint;
        private AgentTrigger _trigger;

        private Vector3 _attackForward;
        private float _comboExpireTime;
        private float _damageMultiplier = 1f;
        private bool _isAttacking;
        private Coroutine _attackRoutine;
        private ComboStep _currentStep;
        
        public int ComboCounter { get; private set; }
        
        public override bool CanFire => !_isAttacking;
        
        public override void InitPart(Weapon weapon, ModuleOwner owner)
        {
            base.InitPart(weapon, owner);

            _damageCaster = GetComponentInChildren<AbstractDamageCaster>();
            _aim = owner.GetModule<IAimModule>();
            _movement = owner.GetModule<IControlMovement>();
            _renderer = owner.GetModule<IRenderer>();
            _constraint = owner.GetModule<ConstraintModule>();
            _trigger = owner.GetModule<AgentTrigger>();
            
            Debug.Assert(_damageCaster != null, $"DamageCaster is null : {gameObject.name}");
            Debug.Assert(_aim != null, $"AimModule is null : {gameObject.name}");
            Debug.Assert(_movement != null, $"Movement is null : {gameObject.name}");
            Debug.Assert(_renderer != null, $"Renderer is null : {gameObject.name}");
            Debug.Assert(_constraint != null, $"Constraint is null : {gameObject.name}");
            Debug.Assert(_trigger != null, $"Trigger is null : {gameObject.name}");

            _damageCaster.InitCaster(owner);
            _constraint.OnConstraintAdded += HandleConstraintAdded;
        }

        private void OnDisable() => CancelAttack();

        private void OnDestroy()
        {
            if (_constraint != null)
                _constraint.OnConstraintAdded -= HandleConstraintAdded;
        }

        public override void Execute(float damageMultiplier)
        {
            _damageMultiplier = damageMultiplier;

            _attackForward = _aim.AimForward;
            _attackForward.y = 0f;
            _attackForward.Normalize();
            
            if (ComboCounter >= comboSteps.Length || Time.time > _comboExpireTime)
                ComboCounter = 0;
            
            _currentStep = comboSteps[ComboCounter];

            _aim.RequestAim(this);
            _renderer.PlayClip(_currentStep.Clip.ParamHash, 0f, 0.05f);

            _isAttacking = true;
            _attackRoutine = StartCoroutine(AttackComboCoroutine());
        }

        private IEnumerator AttackComboCoroutine()
        {
            _trigger.OnAnimationEnd += HandleAnimationEnd;
            _trigger.OnDamageCast += HandleDamageCast;

            AnimationCurve comboCurve = _currentStep.Curve;
            float comboDuration = Mathf.Max(_currentStep.Duration, 0.01f);
            float currentDuration = 0f;
            
            _constraint.Add(ConstraintType.Rooted, this);
            WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

            while (_isAttacking && currentDuration < comboDuration)
            {
                float percent = currentDuration / comboDuration;
                currentDuration += Time.fixedDeltaTime;
                
                float force = comboCurve.Evaluate(percent);
                _movement.SetDrivenVelocity(_attackForward * force);
                
                yield return waitForFixedUpdate;
            }

            if (_isAttacking)
                HandleAnimationEnd();

            EndAttack();
        }

        private void EndAttack()
        {
            _attackRoutine = null;
            _constraint.Remove(ConstraintType.Rooted, this);
            _trigger.OnDamageCast -= HandleDamageCast;
            _trigger.OnAnimationEnd -= HandleAnimationEnd;
            _aim.ReleaseAim(this);
        }

        private void CancelAttack()
        {
            if (_attackRoutine == null) return;

            _isAttacking = false;
            ComboCounter = 0;
            
            StopCoroutine(_attackRoutine);
            EndAttack();
        }

        private void HandleDamageCast()
        {
            _damageCaster.CastDamage(_damageCaster.transform.position, _attackForward,
                                    _weapon.Data.ToDamageSource(_damageMultiplier * _currentStep.DamageMultiplier));
        }

        private void HandleAnimationEnd()
        {
            _isAttacking = false;
            ComboCounter++;
            _comboExpireTime = Time.time + comboWindow;
        }

        private void HandleConstraintAdded(ConstraintType type)
        {
            if (type == ConstraintType.Stunned || type == ConstraintType.Disarmed)
                CancelAttack();
        }
    }
}