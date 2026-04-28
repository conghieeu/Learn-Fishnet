using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace MellowAbelson.Player.Inventory
{
    public class HotbarController : NetworkBehaviour
    {
        [SyncVar] private int _selectedSlotIndex;

        public int SelectedSlotIndex => _selectedSlotIndex;

        [ServerRpc]
        public void ServerSelectSlot(int index)
        {
            _selectedSlotIndex = Mathf.Clamp(index, 0, GetComponent<PlayerInventory>()?.HotbarSlots - 1 ?? 4);
        }

        [ServerRpc]
        public void ServerScroll(int direction)
        {
            int maxSlots = GetComponent<PlayerInventory>()?.HotbarSlots - 1 ?? 4;
            _selectedSlotIndex = (_selectedSlotIndex + direction + maxSlots + 1) % (maxSlots + 1);
        }
    }
}
