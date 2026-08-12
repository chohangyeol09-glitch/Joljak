using CHG.Scripts.Players.InteractionSystem;
using UnityEngine;
using UnityEngine.Events;

namespace CHG.Scripts.Test
{
    public class TestInteractable : MonoBehaviour, IInteractable
    {
        public UnityEvent OnFocusEntered;
        public UnityEvent OnFocusExited;
        public UnityEvent OnInteracted;
        public UnityEvent OnPerformed;
        public UnityEvent OnCancelled;
        
        public void OnFocusEnter()
        {
            Debug.Log($"범위에 들어옴 : {gameObject.name}");
            OnFocusEntered?.Invoke();
        }

        public void OnFocusExit()
        {
            Debug.Log($"범위에 나감 : {gameObject.name}");
            OnFocusExited?.Invoke();
        }

        public void Interact()
        {
            Debug.Log($"Interacted : {gameObject.name}");
            OnInteracted?.Invoke();
        }

        public void Performed()
        {
            Debug.Log($"Performed : {gameObject.name}");
            OnPerformed?.Invoke();
        }

        public void Cancel()
        {
            Debug.Log($"Cancelled : {gameObject.name}");
            OnCancelled?.Invoke();
        }
    }
}