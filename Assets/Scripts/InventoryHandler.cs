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
        // Khởi tạo danh sách slot trống khi server bắt đầu
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

        // BƯỚC 1: Tìm slot đã có vật phẩm này và chưa đầy để cộng dồn (Stacking)
        for (int i = 0; i < Slots.Count; i++)
        {
            if (Slots[i].ItemID == id && Slots[i].Amount < metadata.MaxStack)
            {
                int canAdd = metadata.MaxStack - Slots[i].Amount;
                int adding = Mathf.Min(canAdd, amount);

                ItemSaveData updatedItem = Slots[i];
                updatedItem.Amount += adding;
                Slots[i] = updatedItem; // Cập nhật SyncList

                amount -= adding;
                if (amount <= 0) return true;
            }
        }

        // BƯỚC 2: Nếu còn dư, tìm slot trống để nhét vào
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

        return amount <= 0; // Trả về true nếu đã nhét hết đồ
    }

    [Server]
    public void RemoveItem(int slotIndex, int amount)
    {
        if (slotIndex < 0 || slotIndex >= Slots.Count || Slots[slotIndex].IsEmpty) return;

        ItemSaveData updatedItem = Slots[slotIndex];
        updatedItem.Amount -= amount;

        if (updatedItem.Amount <= 0)
        {
            Slots[slotIndex] = ItemSaveData.Empty();
        }
        else
        {
            Slots[slotIndex] = updatedItem;
        }
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

    #region LƯU VÀ TẢI (EASY SAVE 3)

    [Server]
    public void SaveInventory()
    {
        List<ItemSaveData> dataToSave = new List<ItemSaveData>(Slots);
        ES3.Save("Inventory_Test", dataToSave);
    }

    [Server]
    public void LoadInventory()
    {
        if (ES3.KeyExists("Inventory_Test"))
        {
            List<ItemSaveData> loadedData = ES3.Load<List<ItemSaveData>>("Inventory_Test");
            
            Slots.Clear();
            foreach (var item in loadedData) Slots.Add(item);
            
            while (Slots.Count < MaxSlots) Slots.Add(ItemSaveData.Empty());
        }
    }

    #endregion
}