using Members.CHG.Scripts.CoreSystem.AnimationSystem;
using UnityEngine;

namespace Members.CHG.Scripts.Agents.FSM
{
    [CreateAssetMenu(fileName = "State", menuName = "Agent/StateData", order = 0)]
    public class StateSO : ScriptableObject
    {
        public string stateName;
        public string className;
        public int assetIndex;
        public AnimParamSO stateParam;
    }
}