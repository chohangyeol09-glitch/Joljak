using DevLib.ModuleSystem;
using UnityEngine;

namespace NKT.FlyingEnemy.Modules
{
    public class Shooter : MonoBehaviour, IModule
    {
        [SerializeField] private Transform firePos;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float aimHeight = 1;
        
        public void Initialize(ModuleOwner owner)
        {
            
        }

        public void Fire(Vector3 target)
        {
            Vector3 aim = target + Vector3.up * aimHeight;
            Vector3 dir = (aim - firePos.position).normalized;
            
            //나중에 풀매니저 싹싹 하기
            var bullet = Instantiate(bulletPrefab, firePos.position, Quaternion.LookRotation(dir));
        }
    }
}