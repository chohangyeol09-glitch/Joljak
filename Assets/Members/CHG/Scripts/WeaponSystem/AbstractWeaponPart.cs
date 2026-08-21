using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG.Scripts.WeaponSystem
{
    public abstract class AbstractWeaponPart : MonoBehaviour
    {
        protected Weapon _weapon;
        protected ModuleOwner _owner;

        public virtual bool CanFire => true;

        public virtual void InitPart(Weapon weapon, ModuleOwner owner)
        {
            _weapon = weapon;
            _owner = owner;
        }
        
        public virtual void UpdatePart() {}
        public virtual void OnFireStart() {}
        public virtual void OnFireStop() {}
        public virtual void OnFired() {}
    }
}