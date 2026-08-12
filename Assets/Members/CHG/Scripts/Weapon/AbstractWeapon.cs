using CHG.Scripts.Agents;
using CHG.Scripts.Weapon.WeaponSO;
using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG.Scripts.Weapon
{
    public abstract class AbstractWeapon : MonoBehaviour, IWeapon, IModule
    {
        [SerializeField] protected AbstractWeaponData data;

        public WeaponDisplayInfo Display => data.Display;
        public int Damage => data.Damage;
        public float FireRate => data.AttackRate;

        public virtual bool CanFire => !_constraint.Has(ConstraintType.Disarmed)
                                && Time.time >= _nextFireTime;

        protected ModuleOwner _owner;
        protected IRenderer _renderer;
        protected ConstraintModule _constraint;
        protected bool _isFiring;
        protected float _nextFireTime;

        public virtual void Initialize(ModuleOwner owner)
        {
            _owner = owner;
            _renderer = owner.GetModule<IRenderer>();
            _constraint = owner.GetModule<ConstraintModule>();

            Debug.Assert(data != null, $"WeaponData is null : {gameObject.name}");
            Debug.Assert(_constraint != null, $"ConstraintModule is null : {gameObject.name}");
            Debug.Assert(_renderer != null, $"Renderer is null : {gameObject.name}");
        }

        public virtual void StartFire() => _isFiring = true;

        public virtual void StopFire() => _isFiring = false;

        protected virtual void Update()
        {
            UpdateWeapon();

            if (!_isFiring || !CanFire) return;

            Fire();
            _nextFireTime = Time.time + 1f / FireRate;
        }

        protected virtual void UpdateWeapon(){}
        protected abstract void Fire();
    }
}
