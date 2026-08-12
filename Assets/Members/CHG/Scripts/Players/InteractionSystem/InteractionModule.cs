using System;
using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG.Scripts.Players.InteractionSystem
{
    public class InteractionModule : MonoBehaviour, IModule
    {
        [SerializeField] private LayerMask whatIsInteractable;
        [SerializeField] private LayerMask whatIsObstacle;
        [SerializeField] private float radius = 2f;
        [SerializeField] private float yOffset = 1f;
        
        private Collider[] _hits = new Collider[10];
        private IInteractable _currentInteractable;
        private IInteractable _activeInteractable;
        private bool _performed = false;
        private Transform _ownerTrm;
        
        
        public void Initialize(ModuleOwner owner)
        {
            _ownerTrm = owner.transform;
        }

        private void Update()
        {
            SetCurrent(FindNearInteractable());
            
        }

        private void SetCurrent(IInteractable next)
        {
            if (ReferenceEquals(_currentInteractable, next)) return;
            if (IsAlive(_currentInteractable))
                _currentInteractable.OnFocusExit();
            
            _currentInteractable = next;
            _currentInteractable?.OnFocusEnter();
        }
        
        public void HandleInteractionStarted()
        {
            _activeInteractable = _currentInteractable;
           _performed = false;
           _activeInteractable?.Interact();
        }

        public void HandleInteractionPerformed()
        {
            if (!IsAlive(_activeInteractable)) return;   
            
            _performed = true;
            _activeInteractable?.Performed();
        }
        
        public void HandleInteractionCanceled()
        {
            if (!_performed && IsAlive(_activeInteractable))
                _activeInteractable?.Cancel();
            
            _performed = false;
            _activeInteractable = null;
        }

        private IInteractable FindNearInteractable()
        {
            Vector3 origin = _ownerTrm.position;
            Vector3 eye = origin + Vector3.up * yOffset;
            int count = Physics.OverlapSphereNonAlloc(origin, radius, _hits, whatIsInteractable,
                QueryTriggerInteraction.Collide);

            
            if (count == _hits.Length)
                Debug.LogWarning($"Interactable 후보가 배열을 가득 채웠습니다 : {gameObject.name}");

            IInteractable nearest = null;
            float nearestSqr = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                Collider col = _hits[i];
                if (!col.TryGetComponent(out IInteractable interactable)) continue;
                
                Vector3 point = col.ClosestPoint(origin);
                float sqr = (point - origin).sqrMagnitude;
                if (sqr >= nearestSqr) continue;

                if (Physics.Linecast(eye, point, whatIsObstacle)) continue;
                
                nearestSqr = sqr;
                nearest = interactable;
            }
            
            return nearest;
        }

        private static bool IsAlive(IInteractable interactable) =>
            interactable is Component component && component != null;
        
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            /*Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_ownerTrm.position, radius);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_ownerTrm.position +  Vector3.up * yOffset, 0.3f );*/
        }

#endif
        
    }
}