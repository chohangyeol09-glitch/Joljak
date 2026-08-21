using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CHG.Scripts.UI
{
    public sealed class RuntimeViewModels : IDisposable
    {
        private readonly List<ScriptableObject> _created = new();

        public T Create<T>(T asset) where T : ScriptableObject
        {
            T clone = Object.Instantiate(asset);
            _created.Add(clone);
            return clone;
        }
        
        public void Dispose()
        {
            foreach (ScriptableObject vm in _created)
                Object.Destroy(vm);
            
            _created.Clear();
        }
    }
}