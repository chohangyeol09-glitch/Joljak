using System;
using DevLib.ObjectPool.Runtime;
using UnityEngine;

namespace Members.CHG.Scripts.Weapon
{
    public class Bullet : PoolableMono
    {
        public PoolItemSO PoolItem { get; set; }
        public GameObject GameObject { get; }

        [SerializeField] private float speed;
        [SerializeField] private float lifeTime;
        [SerializeField] private LayerMask hitMask;
        [SerializeField] private PoolManagerSO poolManager;

        private float _despawnTime;

        public void Launch(Vector3 position, Vector3 direction)
        {
            transform.SetPositionAndRotation(position, Quaternion.LookRotation(direction));
            _despawnTime = Time.time + lifeTime;
        }

        private void Update()
        {
            float step = speed * Time.deltaTime;
            transform.position += transform.forward * step;

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, step, hitMask))
            {
                transform.position = hit.point;
                Despawn();
                return;
            }
        }

        private void Despawn()
        {
            poolManager.Push(this);
        }

    }
}