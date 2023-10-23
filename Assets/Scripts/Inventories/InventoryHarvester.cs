using HCGame.Attribute;
using HCGame.Environment;
using HCGame.Saving;
using HCGame.Utils.Static;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.Saving.Json;
using Zenject;

namespace HCGame.Inventories
{
    [RequireComponent(typeof(Inventory))]
    public class InventoryHarvester : MonoBehaviour
    {
        private Inventory _inventory;
        private JsonSavingWrapper _savingWrapper;

        [Inject]
        private void Construct(JsonSavingWrapper savingSystemWrapper)
        {
            _savingWrapper = savingSystemWrapper;
        }

        private void Awake()
        {
            _inventory = GetComponent<Inventory>();
        }

        private void Start()
        {
            CropTile.CropHarvested += OnCropHarvested;
        }

        private void OnDestroy()
        {
            CropTile.CropHarvested -= OnCropHarvested;
        }

        private void OnCropHarvested(CropObject cropObject, int count)
        {
            _inventory.AddItem(cropObject, count);
            _savingWrapper.SendSignalToSave();
        }
    }
}
