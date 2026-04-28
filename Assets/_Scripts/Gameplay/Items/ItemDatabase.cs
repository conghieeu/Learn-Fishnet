using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MellowAbelson.Gameplay.Items
{
    public class ItemDatabase : MonoBehaviour
    {
        [SerializeField] private List<ItemDataSO> _items;

        private Dictionary<int, ItemDataSO> _itemLookup;

        private void Awake()
        {
            _itemLookup = _items.ToDictionary(i => i.ItemId, i => i);
        }

        public ItemDataSO GetItem(int itemId)
        {
            _itemLookup.TryGetValue(itemId, out ItemDataSO item);
            return item;
        }

        public bool TryGetItem(int itemId, out ItemDataSO item)
        {
            return _itemLookup.TryGetValue(itemId, out item);
        }
    }
}
