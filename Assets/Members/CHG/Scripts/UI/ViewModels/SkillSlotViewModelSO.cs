using UnityEngine;
using UnityEngine.UIElements;

namespace CHG.Scripts.UI.ViewModels
{
    /// <summary>
    /// 스킬 슬롯 하나의 표시용 데이터.
    /// current/max 형태가 아니므로 AbstractCountViewModelSO 를 상속하지 않는다.
    ///
    /// 쿨타임 진행도는 여기에 두지 않는다.
    /// 매 프레임 변하는 연속값이라 PlayerHudModule 이 애니메이션으로 직접 그린다.
    /// (재장전 게이지와 같은 방식)
    /// </summary>
    [CreateAssetMenu(fileName = "Skill slot view model", menuName = "UI/Skill slot view", order = 2)]
    public class SkillSlotViewModelSO : ScriptableObject
    {
        [Tooltip("스킬 이름")]
        public string labelName;

        [Tooltip("슬롯 키. 코드가 슬롯 번호로 채운다")]
        public string keyLabel;

        public Sprite icon;

        [Space]
        [Tooltip("슬롯에 스킬이 들어있나. false 면 슬롯 전체가 숨겨진다")]
        public bool isUnlocked;

        [Tooltip("지금 쓸 수 있나. false 면 아이콘이 회색이 된다")]
        public bool isUsable = true;

        /// 슬롯에 스킬이 들어옴 (시작 시 또는 인게임 해금)
        public void SetSkill(string skillName, Sprite skillIcon)
        {
            labelName = skillName;
            icon = skillIcon;
            isUnlocked = true;
        }

        /// 슬롯이 비워짐
        public void ClearSkill()
        {
            labelName = string.Empty;
            icon = null;
            isUnlocked = false;
            isUsable = false;
        }

        public void SetUsable(bool usable) => isUsable = usable;

        // 스킬에만 의미가 있는 컨버터. 범용 컨버터는 CommonUIConverters 에 있다.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RegisterConverters()
        {
            var tintGroup = new ConverterGroup("Bool to Skill Tint");

            tintGroup.AddConverter((ref bool usable) =>
                new StyleColor(usable ? Color.white : new Color(0.34f, 0.34f, 0.40f, 1f)));

            ConverterGroups.RegisterConverterGroup(tintGroup);
        }
    }
}
