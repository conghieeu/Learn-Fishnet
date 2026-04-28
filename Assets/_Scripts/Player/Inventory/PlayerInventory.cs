using FishNet.Object;
using FishNet.Object.Synchronizing;
using MellowAbelson.Core.Save;
using UnityEngine;

namespace MellowAbelson.Player.Inventory
{
    public class PlayerInventory : NetworkBehaviour, ISaveable
    {
        public readonly SyncList<InventorySlotData> Slots = new();
        [SerializeField] private int _maxSlots = 20;
        [SerializeField] private int _hotbarSlots = 4;

        public string SaveKey => $"PlayerInventory_{OwnerId}";
        public int MaxSlots => _maxSlots;
        public int HotbarSlots => _hotbarSlots;

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();
            if (IsServerInitialized)
            {
                for (int i = 0; i < _maxSlots; i++)
                {
                    Slots.Add(new InventorySlotData());
                }
            }
        }

        [ServerRpc]
        public void ServerPickupItem(int itemId, int count)
        {
            for (int i = 0; i < Slots.Count; i++)
            {
                var slot = Slots[i];
                if (slot.IsEmpty)
                {
                    Slots[i] = new InventorySlotData(itemId, count);
                    return;
                }
            }
            Debug.LogWarning("Inventory full!");
        }

        [ServerRpc]
        public void ServerDropItem(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= Slots.Count) return;
            Slots[slotIndex] = new InventorySlotData();
        }

        [ServerRpc]
        public void ServerMoveItem(int fromSlot, int toSlot)
        {
            if (fromSlot < 0 || fromSlot >= Slots.Count) return;
            if (toSlot < 0 || toSlot >= Slots.Count) return;

            var temp = Slots[fromSlot];
            Slots[fromSlot] = Slots[toSlot];
            Slots[toSlot] = temp;
        }

        public object CaptureState()
        {
            var slotArray = new InventorySlotData[Slots.Count];
            for (int i = 0; i < Slots.Count; i++)
                slotArray[i] = Slots[i];
            return slotArray;
        }

        public void RestoreState(object state)
        {
            if (state is InventorySlotData[] slotArray)
            {
                Slots.Clear();
                foreach (var slot in slotArray)
                    Slots.Add(slot);
            }
        }
    }
}
