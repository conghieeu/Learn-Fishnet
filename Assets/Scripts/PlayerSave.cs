using FishNet.Object;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Script đơn giản để test Save/Load vị trí và Inventory.
/// Không cần ID hay Slot phức tạp.
/// </summary>
public class PlayerSave : NetworkBehaviour
{
    private InventoryHandler inventory;

    private void Awake()
    {
        inventory = GetComponent<InventoryHandler>();
    }

    [ContextMenu("Save Everything")]
    public void Save()
    {
        // 1. Lưu vị trí & Rotation
        ES3.Save("Test_Pos", transform.position);
        ES3.Save("Test_Rot", transform.rotation);

        // 2. Lưu Inventory
        if (inventory != null)
        {
            inventory.SaveInventory();
        }

        Debug.Log("✅ [TestSave] Đã lưu Vị trí và Inventory!");
    }

    [ContextMenu("Load Everything")]
    public void Load()
    {
        // 1. Tải vị trí & Rotation
        if (ES3.KeyExists("Test_Pos"))
        {
            transform.position = ES3.Load<Vector3>("Test_Pos");
            transform.rotation = ES3.Load<Quaternion>("Test_Rot");
        }

        // 2. Tải Inventory
        if (inventory != null)
        {
            inventory.LoadInventory();
        }

        Debug.Log("✅ [TestSave] Đã tải Vị trí và Inventory!");
    }
}
