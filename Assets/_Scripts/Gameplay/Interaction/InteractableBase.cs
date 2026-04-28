using FishNet.Object;
using MellowAbelson.Player.Interaction;
using UnityEngine;

namespace MellowAbelson.Gameplay.Interaction
{
    public abstract class InteractableBase : NetworkBehaviour, IInteractable
    {
        [SerializeField] private string _interactionPrompt = "Interact";
        [SerializeField] private float _interactionRange = 3f;

        public string InteractionPrompt => _interactionPrompt;
        public float InteractionRange => _interactionRange;

        public virtual bool CanInteract(GameObject interactor)
        {
            return true;
        }

        public abstract void OnInteract(GameObject interactor);

        public virtual void OnInteractHold(GameObject interactor, float holdProgress) { }

        public virtual void OnInteractCancel(GameObject interactor) { }
    }
}
