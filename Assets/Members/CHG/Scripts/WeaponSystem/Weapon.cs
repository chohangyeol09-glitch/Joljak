using CHG.Scripts.Agents;
using CHG.Scripts.WeaponSystem.WeaponSO;
using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG.Scripts.WeaponSystem
{
    public class Weapon : MonoBehaviour, IModule
    {
        [SerializeField] private WeaponDataSO data;

        private AbstractWeaponPart[] _parts;
        private WeaponDelivery _delivery;
        private ConstraintModule _constraint;
        private bool _isFiring;
        private float _nextFireTime;

        private AbstractWeaponPart[] Parts => _parts ??= GetComponentsInChildren<AbstractWeaponPart>(true);
        
        public WeaponDataSO Data => data;
        public WeaponDisplayInfo Display => Data.Display;
        public bool IsFiring => _isFiring;

        public bool CanFire
        {
            get
            {
                if (_constraint.Has(ConstraintType.Disarmed)) return false;
                if (Time.time < _nextFireTime) return false;
                
                foreach (AbstractWeaponPart part in Parts)
                    if (!part.CanFire) return false;

                return true;
            }
        }
        public void Initialize(ModuleOwner owner)
        {
            _constraint = owner.GetModule<ConstraintModule>();
            _delivery = GetPart<WeaponDelivery>();
            
            Debug.Assert(data != null, $"WeaponData is null : {gameObject.name}");
            Debug.Assert(_constraint != null, $"Constraint is null : {gameObject.name}");
            Debug.Assert(_delivery != null, $"WeaponDelivery is null : {gameObject.name}");
            
            foreach (AbstractWeaponPart part in Parts)
                part.InitPart(this, owner);
        }

        public T GetPart<T>() where T : class
        {
            foreach (AbstractWeaponPart part in Parts)
                if (part is T typed) return typed;
            
            return null;
        }

        public void StartFire()
        {
            _isFiring = true;
            foreach (AbstractWeaponPart part in Parts) part.OnFireStart();
        }

        public void StopFire()
        {
            _isFiring = false;
            foreach (AbstractWeaponPart part in Parts) part.OnFireStop();
        }

        public void RequestFire(float damageMultiplier = 1f)
        {
            if (!CanFire) return;
            DoFire(damageMultiplier);
        }

        public void StopAutoFire() => _isFiring = false;

        private void Update()
        {
            foreach (AbstractWeaponPart part in Parts) part.UpdatePart();

            if (!_isFiring || !CanFire) return;

            DoFire(1f);
        }

        private void DoFire(float damageMultiplier)
        {
            _delivery.Execute(damageMultiplier);
            _nextFireTime = Time.time + 1f / Mathf.Max(data.AttackRate, 0.01f);
            
            foreach (AbstractWeaponPart part in Parts) part.OnFired();
        }
    }
}