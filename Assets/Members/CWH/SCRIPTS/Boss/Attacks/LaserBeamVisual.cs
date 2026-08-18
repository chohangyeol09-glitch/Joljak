using UnityEngine;

namespace Boss.Attacks
{
    public class LaserBeamVisual : MonoBehaviour
    {
        [Header("Particle Beam (레이져 빔)")]
        [SerializeField] private ParticleSystem particleBeam;
        [SerializeField] private float particleSpeed = 20f;

        private Transform originalParent;
        private int originalSiblingIndex;

        private void Awake()
        {
            if (particleBeam != null)
            {
                particleBeam.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                originalParent = particleBeam.transform.parent;
                originalSiblingIndex = particleBeam.transform.GetSiblingIndex();
            }
        }

        public void ShowBeam(Vector3 origin, Vector3 endPoint)
        {
            if (particleBeam != null)
            {
                // 발사 중엔 본 애니메이션에 끌려다니지 않도록 계층에서 잠시 분리한다.
                particleBeam.transform.SetParent(null, true);
            }

            UpdateBeam(origin, endPoint);
            particleBeam?.Play();
        }

        public void UpdateBeam(Vector3 origin, Vector3 endPoint)
        {
            if (particleBeam == null)
            {
                return;
            }

            Vector3 toEnd = endPoint - origin;
            float distance = toEnd.magnitude;
            if (distance < 0.0001f)
            {
                return;
            }

            particleBeam.transform.position = origin;
            particleBeam.transform.rotation = Quaternion.LookRotation(toEnd);

            // 파티클이 정확히 목표 지점까지 이동한 뒤 사라지도록 수명을 거리에 맞게 계산
            var main = particleBeam.main;
            main.startSpeed = particleSpeed;
            main.startLifetime = distance / particleSpeed;
        }

        public void HideBeam()
        {
            particleBeam?.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
