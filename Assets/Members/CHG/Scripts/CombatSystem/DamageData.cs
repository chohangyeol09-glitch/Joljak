using System.Numerics;
using Members.CHG.Scripts.CoreSystem.ModuleSystem;

namespace Members.CHG.Scripts.CombatSystem
{
    public struct DamageData
    {
        public float DamageAmount;
        public Vector3 HitPoint;
        public Vector3 HitNormal;
        public ModuleOwner Attacker;
        public bool IsCritical;

        public DamageData(float damageAmount, Vector3 hitPoint, Vector3 hitNormal, ModuleOwner attacker, bool isCritical)
        {
            DamageAmount = damageAmount;
            HitPoint = hitPoint;
            HitNormal = hitNormal;
            Attacker = attacker;
            IsCritical = isCritical;
        }
    }
}