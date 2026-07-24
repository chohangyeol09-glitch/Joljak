using UnityEngine;

namespace Members.CHG.Scripts.Agents.FSM
{
    [CreateAssetMenu(fileName = "StateListData", menuName = "Agent/State list", order = 0)]
    public class StateListSO : ScriptableObject
    {
        [HideInInspector] public string generatePath;
        public string enumName;
        public StateSO[] states;
    }
}