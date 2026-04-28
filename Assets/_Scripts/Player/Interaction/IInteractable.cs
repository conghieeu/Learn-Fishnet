using UnityEngine;

namespace MellowAbelson.Player.Interaction
{
    public interface IInteractable
    {
        string InteractionPrompt { get; }
        float InteractionRange { get; }
        bool CanInteract(GameObject interactor);
        void OnInteract(GameObject interactor);
        void OnInteractHold(GameObject interactor, float holdProgress);
        void OnInteractCancel(GameObject interactor);
    }
}
