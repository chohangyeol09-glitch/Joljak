using System;
using System.Collections;
using CHG.Scripts.Agents;
using CHG.Scripts.CombatSystem;
using CHG.Scripts.CoreSystem.AnimationSystem;
using CHG.Scripts.Players;
using CHG.Scripts.Weapon.WeaponSO;
using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG.Scripts.Weapon
{
    public class MeleeWeapon : AbstractWeapon
    {

        private MeleeWeaponSO _data;
        private AbstractDamageCaster _damageCasters;
        private IAimModule _aim;
        private IControlMovement _movement;
        private AgentTrigger _trigger;
        private Vector3 _attackForward;
        private float _comboExpireTime;
        private bool _isAttacking;
        private Coroutine _attackRoutine;
        private MeleeWeaponSO.ComboStep _currentStep;
        
        public override bool CanFire => base.CanFire && !_isAttacking;
        public int ComboCounter { get; set; } = 0;
        

        public override void Initialize(ModuleOwner owner)
        {
            base.Initialize(owner);
            _data = data as MeleeWeaponSO;
            _damageCasters = GetComponentInChildren<AbstractDamageCaster>();
            _movement = owner.GetModule<IControlMovement>();
            _trigger = owner.GetModule<AgentTrigger>();
            _aim = owner.GetModule<IAimModule>();

            Debug.Assert(_data != null, $"MeleeWeaponData is null : {gameObject.name}");
            Debug.Assert(_damageCasters != null, $"DamageCasters is null : {gameObject.name}");
            Debug.Assert(_movement != null, $"Movement is null : {gameObject.name}");
            Debug.Assert(_trigger != null, $"Trigger is null : {gameObject.name}");
            Debug.Assert(_aim != null, $"Aim is null : {gameObject.name}");

            _damageCasters.InitCaster(_owner);
            _constraint.OnConstraintAdded += HandleConstraintAdded;
        }

        private void OnDisable()
        {
            CancelAttack();
        }

        private void OnDestroy()
        {
            _constraint.OnConstraintAdded -= HandleConstraintAdded;
        }

        protected override void Fire()
        {
            bool comboCounterOver = ComboCounter >= _data.ComboSteps.Length;
            bool comboWindowExhausted = Time.time > _comboExpireTime;
            _attackForward = _aim.AimForward;
            _attackForward.y = 0f;
            _attackForward.Normalize();
            if (comboCounterOver || comboWindowExhausted)
            {
                ComboCounter = 0;
            }

            _currentStep = _data.ComboSteps[ComboCounter];
            _renderer.PlayClip(_currentStep.Clip.ParamHash, 0f,0.05f);

            _isAttacking = true;
            _attackRoutine = StartCoroutine(AttackComboCoroutine());
                _aim.RequestAim(this);        
        }

        private IEnumerator AttackComboCoroutine()
        {
            _trigger.OnAnimationEnd += HandleAnimationEnd;
            _trigger.OnDamageCast += HandleDamageCaster;

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
            _trigger.OnDamageCast -= HandleDamageCaster;
            _trigger.OnAnimationEnd -= HandleAnimationEnd;
            _aim.ReleaseAim(this);
        }

        private void CancelAttack()
        {
            if (!_isAttacking) return;
            
            _isAttacking = false;
            ComboCounter = 0;
            
            if (_attackRoutine != null) StopCoroutine(_attackRoutine);
            EndAttack();
        }

        private void HandleDamageCaster()
        {
            _damageCasters.CastDamage(
                _damageCasters.transform.position,
                _attackForward,
                _data.ToDamageSource(_currentStep.DamageMultiplier));
        }

        private void HandleAnimationEnd()
        {
            _isAttacking = false;
            ComboCounter++;
            _comboExpireTime = Time.time + _data.ComboWindow;
        }
        
        private void HandleConstraintAdded(ConstraintType type)
        {
            if (type == ConstraintType.Stunned || type == ConstraintType.Disarmed)
                CancelAttack();
        }

    }
}