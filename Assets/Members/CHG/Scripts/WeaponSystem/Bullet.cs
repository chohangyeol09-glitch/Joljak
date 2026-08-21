using CHG.Scripts.CombatSystem;
using DevLib.ObjectPool.Runtime;
using UnityEngine;

namespace CHG.Scripts.WeaponSystem
{
    public class Bullet : PoolableMono
    {
        [SerializeField] private float speed;
        [SerializeField] private float lifeTime;
        [SerializeField] private float radius;
        [SerializeField] private LayerMask hitMask;
        [SerializeField] private PoolManagerSO poolManager;
        
        [Header("Homing")]
        [SerializeField] private float turnRate = 360f;
        [SerializeField] private float aimHeight = 1f;

        private Transform _homingTarget;
        private float _despawnTime;
        private bool _isDespawned;
        private DamageData _damageData;

        public void Launch(Vector3 position, Vector3 direction, DamageData damageData, Transform homingTarget = null)
        {
            transform.SetPositionAndRotation(position, Quaternion.LookRotation(direction));
            _despawnTime = Time.time + lifeTime;
            _damageData = damageData;
            _homingTarget = homingTarget;
            _isDespawned = false;
        }

        private void Update()
        {
            if (_homingTarget != null && _homingTarget.gameObject.activeInHierarchy)
            {
                Vector3 desired = (_homingTarget.position + Vector3.up * aimHeight) - transform.position;
                transform.forward = Vector3.RotateTowards(
                    transform.forward, desired.normalized,
                    turnRate * Mathf.Deg2Rad * Time.deltaTime, 0f);
            }

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
            
            _damageData.HitPoint = hit.point;
            _damageData.HitNormal = hit.normal;
            damageable?.ApplyDamage(_damageData);
            
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
            _homingTarget = null;
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