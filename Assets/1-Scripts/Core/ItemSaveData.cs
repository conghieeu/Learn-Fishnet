[System.Serializable]
public struct ItemSaveData
{
    public string ItemID;
    public int Amount;

    // Kiểm tra xem slot có trống không
    public bool IsEmpty => string.IsNullOrEmpty(ItemID);

    public static ItemSaveData Empty() => new ItemSaveData { ItemID = "", Amount = 0 };
}