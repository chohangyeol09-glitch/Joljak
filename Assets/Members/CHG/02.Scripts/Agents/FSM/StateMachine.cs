using System;
using System.Collections.Generic;
using UnityEngine;

namespace Members.CHG._02.Scripts.Agents.FSM
{
    public class StateMachine
    {
        public AgentState CurrentState { get; private set; }
        private Dictionary<int, AgentState> _stateDict;

        public StateMachine(Agent agent, StateSO[] stateList)
        {
            _stateDict = new Dictionary<int, AgentState>();
            foreach (StateSO stateData in stateList)
            {
                Type type = Type.GetType(stateData.className);
                Debug.Assert(type != null, $"찾고자 하는 타입이 없습니다. : {stateData.className}");
                AgentState state = (AgentState)Activator.CreateInstance(type);
                
                _stateDict.Add(stateData.assetIndex, state);
            }
        }

        public void ChangeState(int newStateIndex, float transitionDuration)
        {
            CurrentState?.Exit();
            AgentState newState = _stateDict.GetValueOrDefault(newStateIndex);

            CurrentState = newState;
            CurrentState.Enter(transitionDuration);
        }
        
        public void UpdateMachine() => CurrentState?.Update();
    }
}