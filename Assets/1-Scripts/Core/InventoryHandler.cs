using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Connection;

public class InventoryHandler : NetworkBehaviour
{
    [Header("Settings")]
    public int MaxSlots = 10;
    public ItemDatabaseSO Database;

    // Danh sách các slot, tự động đồng bộ từ Server xuống Client
    public readonly SyncList<ItemSaveData> Slots = new SyncList<ItemSaveData>();

    public override void OnStartServer()
    {
        base.OnStartServer();
        // Chỉ khởi tạo slot trống nếu chưa được nạp data từ save
        if (Slots.Count == 0)
        {
            for (int i = 0; i < MaxSlots; i++) Slots.Add(ItemSaveData.Empty());
        }
    }

    #region LOGIC XỬ LÝ (CHỈ CHẠY TRÊN SERVER)

    [Server]
    public bool AddItem(string id, int amount)
    {
        ItemSO metadata = Database.GetItemByID(id);
        if (metadata == null) return false;

        // Cộng dồn vào slot đã có
        for (int i = 0; i < Slots.Count; i++)
        {
            if (Slots[i].ItemID == id && Slots[i].Amount < metadata.MaxStack)
            {
                int canAdd = metadata.MaxStack - Slots[i].Amount;
                int adding = Mathf.Min(canAdd, amount);

                ItemSaveData updatedItem = Slots[i];
                updatedItem.Amount += adding;
                Slots[i] = updatedItem;

                amount -= adding;
                if (amount <= 0) return true;
            }
        }

        // Nhét vào slot trống
        for (int i = 0; i < Slots.Count; i++)
        {
            if (Slots[i].IsEmpty)
            {
                int adding = Mathf.Min(metadata.MaxStack, amount);
                Slots[i] = new ItemSaveData { ItemID = id, Amount = adding };

                amount -= adding;
                if (amount <= 0) return true;
            }
        }

        return amount <= 0;
    }

    [Server]
    public void RemoveItem(int slotIndex, int amount)
    {
        if (slotIndex < 0 || slotIndex >= Slots.Count || Slots[slotIndex].IsEmpty) return;

        ItemSaveData updatedItem = Slots[slotIndex];
        updatedItem.Amount -= amount;

        Slots[slotIndex] = updatedItem.Amount <= 0 ? ItemSaveData.Empty() : updatedItem;
    }

    [Server]
    public void SwapItems(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= Slots.Count || indexB < 0 || indexB >= Slots.Count) return;

        ItemSaveData temp = Slots[indexA];
        Slots[indexA] = Slots[indexB];
        Slots[indexB] = temp;
    }

    #endregion

    #region NẠP DATA TỪ SAVE

    /// <summary>
    /// Nạp dữ liệu inventory từ save (gọi bởi Player.cs sau khi nhận tên).
    /// </summary>
    [Server]
    public void SetSlots(List<ItemSaveData> savedItems)
    {
        Slots.Clear();
        foreach (var item in savedItems)
            Slots.Add(item);

        while (Slots.Count < MaxSlots) Slots.Add(ItemSaveData.Empty());

        Debug.Log($"[Inventory] Đã nạp {savedItems.Count} slots từ save.");
    }

    #endregion
}