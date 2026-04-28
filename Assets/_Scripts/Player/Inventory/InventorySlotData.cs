using System;

namespace MellowAbelson.Player.Inventory
{
    [Serializable]
    public struct InventorySlotData
    {
        public int ItemId;
        public int StackCount;
        public float CurrentDurability;

        public InventorySlotData(int itemId, int stackCount = 1, float durability = 100f)
        {
            ItemId = itemId;
            StackCount = stackCount;
            CurrentDurability = durability;
        }

        public bool IsEmpty => ItemId == 0;
    }
}
