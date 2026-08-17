using CHG.Scripts.Agents.StatSystem;
using CHG.Scripts.SkillSystem;
using CHG.Scripts.Weapon.WeaponSO;
using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG.Scripts.CombatSystem
{
    public abstract class AbstractDamageCaster : MonoBehaviour
    {
        [SerializeField] protected LayerMask whatIsEnemy;
        
        public ModuleOwner CasterOwner { get; private set; }
        public Vector3 LastHitPosition { get; protected set; }
        public Vector3 LashHitNormal { get; protected set; }
        public bool LastHitCritical { get; protected set; }
        public IStatModule StatModule { get; protected set; }
        
        public virtual void InitCaster(ModuleOwner owner)
        {   
            CasterOwner = owner;
            StatModule = CasterOwner.GetModule<IStatModule>();
        }

        public abstract bool CastDamage(Vector3 position, Vector3 direction, DamageSource source);
    }
}