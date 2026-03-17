using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Database")]
public class ItemDatabaseSO : ScriptableObject
{
    public List<ItemSO> AllItems;

    public ItemSO GetItemByID(string id)
    {
        return AllItems.Find(x => x.ItemID == id);
    }
}

