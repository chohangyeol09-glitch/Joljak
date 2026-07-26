using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Boss.Attacks
{
    public class Missile : MonoBehaviour
    {
        [SerializeField] private float launchForwardDistance = 2f;
        [SerializeField] private float launchPhaseDuration = 0.25f;
        [SerializeField] private float apexHeight = 6f;
        [SerializeField] private float ascendDuration = 1f;
        [SerializeField] private float descendDuration = 0.4f;

        public void Launch(Vector3 impactPoint, Vector3 launchDirection, AttackTelegraph telegraph, float blinkLeadTime, Action onImpact = null)
        {
            float totalDuration = launchPhaseDuration + ascendDuration + descendDuration;

            if (telegraph != null)
            {
                float blinkDelay = Mathf.Max(totalDuration - blinkLeadTime, 0f);
                DOVirtual.DelayedCall(blinkDelay, () => telegraph.StartBlinking(totalDuration - blinkDelay));
            }

            Vector3 launchEndPoint = transform.position + launchDirection.normalized * launchForwardDistance;
            StartCoroutine(FlightRoutine(transform.position, launchEndPoint, impactPoint, telegraph, onImpact));
        }

        private IEnumerator FlightRoutine(Vector3 origin, Vector3 launchEndPoint, Vector3 impactPoint, AttackTelegraph telegraph, Action onImpact)
        {
            Vector3 previousPosition = origin;

            // 1단계: 완전 수평 직진 (아직 안 올라감)
            float dashElapsed = 0f;
            while (dashElapsed < launchPhaseDuration)
            {
                dashElapsed += Time.deltaTime;
                float dashT = Mathf.Clamp01(dashElapsed / launchPhaseDuration);

                Vector3 position = Vector3.Lerp(origin, launchEndPoint, dashT);
                FaceDirection(position - previousPosition);
                transform.position = position;
                previousPosition = position;

                yield return null;
            }

            // 2단계: 포물선 (여기서부터 위로 솟았다가 내리꽂힘)
            float arcDuration = ascendDuration + descendDuration;
            float ascendFraction = ascendDuration / arcDuration;
            float arcElapsed = 0f;

            while (arcElapsed < arcDuration)
            {
                arcElapsed += Time.deltaTime;
                float timeFraction = Mathf.Clamp01(arcElapsed / arcDuration);

                float t = timeFraction < ascendFraction
                    ? Mathf.SmoothStep(0f, 0.5f, timeFraction / ascendFraction)
                    : 0.5f + Mathf.SmoothStep(0f, 0.5f, (timeFraction - ascendFraction) / (1f - ascendFraction));

                Vector3 flatPosition = Vector3.Lerp(launchEndPoint, impactPoint, t);
                float arc = 4f * apexHeight * t * (1f - t);
                Vector3 position = flatPosition + Vector3.up * arc;

                FaceDirection(position - previousPosition);
                transform.position = position;
                previousPosition = position;

                yield return null;
            }

            transform.position = impactPoint;

            if (telegraph != null)
            {
                Destroy(telegraph.gameObject);
            }

            onImpact?.Invoke();
            Destroy(gameObject);
        }

        private void FaceDirection(Vector3 direction)
        {
            if (direction.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
}
