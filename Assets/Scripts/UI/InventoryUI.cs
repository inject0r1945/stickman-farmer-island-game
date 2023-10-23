using HCGame.Attribute;
using HCGame.Environment;
using HCGame.Inventories;
using HCGame.Utils.Static;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HCGame.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private CropContainerUI _cropContainerPrefab;

        private Inventory _inventory;

        private void Awake()
        {
            _inventory = Player.CurrentInventory;
            ClearCropContainersUI();
        }

        private void OnEnable()
        {
            _inventory.InventoryChanged += OnInventoryChanged;
        }

        private void OnDisable()
        {
            _inventory.InventoryChanged -= OnInventoryChanged;
        }

        private void ClearCropContainersUI()
        {
            foreach (Transform container in transform)
            {
                Destroy(container.gameObject);
            }
        }

        private void OnInventoryChanged(CropObject cropObject, int amount)
        {
            CropContainerUI cropContainer = GetComponentsInChildren<CropContainerUI>()
                .Where(x => x.CropType == cropObject.CropType)
                .FirstOrDefault();

            if (amount == 0 && cropContainer == null)
                return;

            if (amount == 0 && cropContainer != null)
            {
                Destroy(cropContainer.gameObject);
                return;
            }

            if (cropContainer == null)
            {
                CreateCropContainer(cropObject, amount);
            }
            else
            {
                cropContainer.Init(cropObject, amount);
            }
        }

        private CropContainerUI CreateCropContainer(CropObject cropObject, int amount)
        {
            CropContainerUI cropContainer = Instantiate(_cropContainerPrefab, transform);
            cropContainer.Init(cropObject, amount);

            return cropContainer;
        }
    }
}
