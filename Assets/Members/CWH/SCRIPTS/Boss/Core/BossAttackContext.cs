using UnityEngine;

namespace Boss.Core
{
    public readonly struct BossAttackContext
    {
        public Transform Self { get; }
        public Transform Target { get; }

        public BossAttackContext(Transform self, Transform target)
        {
            Self = self;
            Target = target;
        }
    }
}
