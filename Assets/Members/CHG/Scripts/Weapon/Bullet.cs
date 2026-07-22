using DevLib.ObjectPool.Runtime;
using UnityEngine;

namespace Members.CHG.Scripts.Weapon
{
    public class Bullet : MonoBehaviour, IPoolable
    {
        public PoolItemSO PoolItem { get; set; }
        public GameObject GameObject { get; }
        public void ResetItem()
        {
            
        }
    }
}