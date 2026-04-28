using UnityEngine;

namespace MellowAbelson.Gameplay.Items
{
    [CreateAssetMenu(menuName = "MellowAbelson/Items/Item Data", fileName = "NewItemData")]
    public class ItemDataSO : ScriptableObject
    {
        public int ItemId;
        public string ItemName;
        public string Description;
        public GameObject WorldPrefab;
        public Sprite Icon;
        public bool IsStackable;
        public int MaxStackSize = 1;
        public float Weight = 1f;
    }
}
