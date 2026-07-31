using System;
using UnityEngine;

namespace CHG.Scripts.UI.ViewModels
{
    public abstract class AbstractCountViewModelSO : ScriptableObject
    {
        public string labelName;
        
        [Space]
        public int current;
        public int max;

        [HideInInspector] public float normalized;

        protected void OnEnable()
        {
            SyncNormalized();
        }
        
#if UNITY_EDITOR
        protected void OnValidate() => SyncNormalized();
#endif

        public void SetValue(int current, int max)
        {
            this.current = current;
            this.max = max;
            SyncNormalized();
        }
        
        protected void SyncNormalized()
        {
            normalized = max > 0 ? Mathf.Clamp01((float)current / max) : 0f;
        }
    }
}