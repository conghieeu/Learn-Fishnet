using UnityEngine;
using System.Collections.Generic;

// Class cho từng Item
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemSO : ScriptableObject
{
    public string ItemID;
    public string ItemName;
    public int MaxStack = 99;
    public GameObject Prefab; // Để spawn ra đất khi vứt đồ
}