using CHG.Scripts.Agents;
using CHG.Scripts.CombatSystem;
using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG.Scripts.WeaponSystem
{
    public class AutoTargetPart : AbstractWeaponPart, IWeaponTargeting
    {
        [SerializeField] private float maxRange = 30f;
        [SerializeField] private LayerMask whatIsTarget;
        [Tooltip("화면에서 잘라낼 비율")]
        [SerializeField, Range(0f,0.45f)] private float viewportMargin;
        [SerializeField] private float distanceTolerance = 2f;
        
        [SerializeField] private bool requireLineOfSight = true;
        [SerializeField] private LayerMask whatIsObstacle;

        private readonly Collider[] _hits = new Collider[32];
        private Camera _cam;

        public override void InitPart(Weapon weapon, ModuleOwner owner)
        {
            base.InitPart(weapon, owner);
            
            _cam = Camera.main;
            Debug.Assert(_cam != null, $"Cam is null : {gameObject.name}");
        }

        public Transform FindTarget(Vector3 origin)
        {
            int count = Physics.OverlapSphereNonAlloc(origin, maxRange, _hits, whatIsTarget,
                QueryTriggerInteraction.Ignore);

            Vector3 eye = _cam.transform.position;

            Transform best = null;
            float bestDistance = 0f;
            float bestAngle = 0f;

            for (int i = 0; i < count; i++)
            {
                Collider col = _hits[i];
                Vector3 point = col.bounds.center;
                
                if (!TryGetScreenOffset(point, out float screenOffset)) continue;
                
                if (col.GetComponentInParent<IDamageable>() is not MonoBehaviour target) continue;

                if (target is Agent agent && agent.IsDead) continue;

                if (requireLineOfSight &&
                    Physics.Linecast(eye, point, whatIsObstacle, QueryTriggerInteraction.Ignore)) continue;
                
                float distance = Vector3.Distance(origin, point);

                if (best != null && !IsBetter(distance, screenOffset, bestDistance, bestAngle))
                    continue;

                best = target.transform;
                bestDistance = distance;
                bestAngle = screenOffset;
            }
            
            return best;

        }

        private bool TryGetScreenOffset(Vector3 point, out float offset)
        {
            offset = 0f;

            Vector3 vp = _cam.WorldToViewportPoint(point);

            if (vp.z <= 0f) return false;
            if (vp.x < viewportMargin || vp.x > 1f - viewportMargin) return false;
            if (vp.y < viewportMargin || vp.y > 1f - viewportMargin) return false;

            offset = new Vector2((vp.x - 0.5f) * _cam.aspect, vp.y - 0.5f).magnitude;
            return true;
        }
        
        private bool IsBetter(float distance, float screenOffset, float bestDistance, float bestScreenOffset)
        {
            if (distance < bestDistance - distanceTolerance) return true;
            if (distance > bestDistance + distanceTolerance ) return false;
            
            return screenOffset < bestScreenOffset;
        }
    }
}