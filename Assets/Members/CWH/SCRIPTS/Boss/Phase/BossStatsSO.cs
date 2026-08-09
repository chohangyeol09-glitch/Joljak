using System.Collections.Generic;
using UnityEngine;

namespace Boss.Phase
{
    [CreateAssetMenu(fileName = "BossStats", menuName = "Boss/Boss Stats")]
    public class BossStatsSO : ScriptableObject
    {
        [Tooltip("체력 비율이 이 값 이하로 떨어질 때마다 다음 페이즈로 전환됩니다. 반드시 내림차순으로 입력하세요. 예: 0.7, 0.35")]
        [SerializeField] private float[] phaseHealthThresholds = { 0.7f, 0.35f };

        public IReadOnlyList<float> PhaseHealthThresholds => phaseHealthThresholds;
    }
}
