using System;
using UnityEditor;
using UnityEngine;

namespace Members.CHG._02.Scripts.Agents.FSM
{
    [CreateAssetMenu(fileName = "State", menuName = "Agent/StateData", order = 0)]
    public class StateSO : ScriptableObject
    {
        public string stateName;
        public string className;
        public int assetIndex;
    }
}