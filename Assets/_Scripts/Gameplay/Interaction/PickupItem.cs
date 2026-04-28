using FishNet.Object;
using MellowAbelson.Player.Inventory;
using UnityEngine;

namespace MellowAbelson.Gameplay.Interaction
{
    public class PickupItem : InteractableBase
    {
        [SerializeField] private int _itemId;
        [SerializeField] private int _itemCount = 1;

        public override string InteractionPrompt => $"Pick up {name}";

        public override void OnInteract(GameObject interactor)
        {
            if (!IsServer) return;

            var inventory = interactor.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.ServerPickupItem(_itemId, _itemCount);
                DespawnServer();
            }
        }

        [Server]
        private void DespawnServer()
        {
            Despawn();
        }
    }
}
