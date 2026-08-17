namespace CHG.Scripts.Players.InteractionSystem
{
    public interface IInteractable
    {
        void OnFocusEnter();
        void OnFocusExit();
        void Interact();
        void Performed();
        void Cancel();
    }
}