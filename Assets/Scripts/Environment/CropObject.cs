using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace HCGame.Environment
{
    [CreateAssetMenu(fileName = "Crop Data", menuName = "Plants/Crop Data")]
    public class CropObject : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private Crop _cropPrefab;
        [SerializeField] private Material _cropMaterial;
        [SerializeField] private CropType _cropType;
        [SerializeField] private Sprite _icon;
        [SerializeField] private int _cost = 1;
        [SerializeField] private string _itemID;

        private static Dictionary<string, CropObject> _itemLookupCache;

        public static CropObject[] AllCropObjects 
        {
            get
            {
                CreateLookupCache();
                return _itemLookupCache.Values.ToArray();
            }
        }

        public Crop CropPrefab => _cropPrefab;

        public Material CropMaterial => _cropMaterial;

        public CropType CropType => _cropType;

        public Sprite Icon => _icon;

        public int Cost => _cost;

        public string ItemID => _itemID;

        public static CropObject GetFromID(string itemID)
        {
            CreateLookupCache();

            if (itemID == null || !_itemLookupCache.ContainsKey(itemID))
                return null;

            return _itemLookupCache[itemID];
        }

        public Crop InstantiateCrop(Vector3 position, Transform parent)
        {
            Crop crop = Instantiate(_cropPrefab, position, Quaternion.identity);
            crop.transform.SetParent(parent);

            return crop;
        }

        private static void CreateLookupCache()
        {
            if (_itemLookupCache == null)
            {
                _itemLookupCache = new Dictionary<string, CropObject>();
                CropObject[] cropObjectsList = UnityEngine.Resources.LoadAll<CropObject>(string.Empty);

                foreach (CropObject cropObject in cropObjectsList)
                {
                    if (_itemLookupCache.ContainsKey(cropObject.ItemID))
                    {
                        Debug.LogError($"Looks like there's a duplicate RPG.UI.InventorySystem ID for objects: {_itemLookupCache[cropObject.ItemID]} and {cropObject}");
                        continue;
                    }

                    _itemLookupCache[cropObject.ItemID] = cropObject;
                }
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (string.IsNullOrWhiteSpace(_itemID))
            {
                _itemID = Guid.NewGuid().ToString();
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
        }
    }
}