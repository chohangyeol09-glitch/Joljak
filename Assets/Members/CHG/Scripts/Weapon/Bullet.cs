using CHG.Scripts.CombatSystem;
using CHG.Scripts.CoreSystem.ModuleSystem;
using DevLib.ObjectPool.Runtime;
using UnityEngine;

namespace CHG.Scripts.Weapon
{
    public class Bullet : PoolableMono
    {
        [SerializeField] private float speed;
        [SerializeField] private float lifeTime;
        [SerializeField] private float radius;
        [SerializeField] private LayerMask hitMask;
        [SerializeField] private PoolManagerSO poolManager;
        
        private float _despawnTime;
        private int _damage;
        private bool _isDespawned;
        private ModuleOwner _attacker;

        public void Launch(Vector3 position, Vector3 direction, int damage, ModuleOwner attacker)
        {
            transform.SetPositionAndRotation(position, Quaternion.LookRotation(direction));
            _despawnTime = Time.time + lifeTime;
            _damage = damage;
            _attacker = attacker;
            _isDespawned = false;
        }

        private void Update()
        {
            float step = speed * Time.deltaTime;

            if (Physics.SphereCast(transform.position, radius, transform.forward,
                    out RaycastHit hit, step, hitMask, QueryTriggerInteraction.Ignore))
            {
                HandleHit(hit);
                return;
            }
            
            transform.position += transform.forward * step;
            if (Time.time > _despawnTime)
            {
                Despawn();
            }
        }

        private void HandleHit(RaycastHit hit)
        {
            if (hit.distance > 0f)
                transform.position = hit.point;
            
            IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
            damageable?.ApplyDamage(new DamageData(_damage, hit.point, hit.normal, _attacker, false));
            
            Despawn();
        }

        private void Despawn()
        {
            if (_isDespawned) return;
            
            _isDespawned = true;
            poolManager.Push(this);
            
        }

        public override void ResetItem()
        {
            base.ResetItem();
            _isDespawned = false;
        }
        
        #if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

#endif
    }
}