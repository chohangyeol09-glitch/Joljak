using DevLib.ModuleSystem;

namespace CHG.Scripts.Players.InteractionSystem
{
    public interface IInteractable
    {
        void OnFocusEnter();
        void OnFocusExit();
        void Interact(ModuleOwner interactor);
        void Performed();
        void Cancel();
    }
}