using HCGame.Environment;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.Saving.Json;
using HCGame.Utils.Static;
using UnityEngine.Events;
using System.Linq;

namespace HCGame.Inventories
{
    public class Inventory : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] private Dictionary<CropType, InventoryItem> _inventoryCache = new Dictionary<CropType, InventoryItem>();

        public InventoryItem[] InventoryItems => _inventoryCache.Values.ToArray();

        public event UnityAction<CropObject, int> InventoryChanged;

        public JToken CaptureAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;

            foreach (InventoryItem inventoryItem in _inventoryCache.Values)
            {
                stateDict.Add(inventoryItem.CropObject.ItemID, JToken.FromObject(inventoryItem.Amount));
            }

            return state;
        }

        public void RestoreFromJToken(JToken state)
        {
            IDictionary<string, JToken> stateDict = (IDictionary<string, JToken>)state;
            _inventoryCache.Clear();

            foreach (string itemId in stateDict.Keys)
            {
                CropObject cropObject = CropObject.GetFromID(itemId);
                AddItem(cropObject, stateDict[itemId].ToObject<int>());
            }
        }

        public void AddItem(CropObject cropObject, int count)
        {
            InventoryItem item = GetItem(cropObject);
            item.IncreaseAmount(count);

            InventoryChanged?.Invoke(cropObject, item.Amount);
        }

        public InventoryItem GetItem(CropObject cropObject)
        {
            if (!_inventoryCache.ContainsKey(cropObject.CropType))
                _inventoryCache.Add(cropObject.CropType, new InventoryItem(cropObject));

            return _inventoryCache[cropObject.CropType];
        }

        public void RemoveItem(InventoryItem inventoryItem)
        {
            RemoveItem(inventoryItem.CropObject);
        }

        public void RemoveItem(CropObject cropObject)
        {
            if (!_inventoryCache.ContainsKey(cropObject.CropType))
                return;

            int totalCount = _inventoryCache[cropObject.CropType].Amount;

            RemoveItem(cropObject, totalCount);
        }

        public void RemoveItem(CropObject cropObject, int count)
        {
            if (!_inventoryCache.ContainsKey(cropObject.CropType))
                return;

            InventoryItem inventoryItem = _inventoryCache[cropObject.CropType];
            inventoryItem.IncreaseAmount(-count);

            InventoryChanged?.Invoke(cropObject, inventoryItem.Amount);
        }
    }
}

