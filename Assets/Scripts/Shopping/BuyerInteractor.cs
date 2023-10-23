using HCGame.Inventories;
using HCGame.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace HCGame.Shopping
{
    [RequireComponent(typeof(Inventory))]
    [RequireComponent(typeof(Wallet))]
    public class BuyerInteractor : MonoBehaviour
    {
        private Inventory _inventory;
        private Wallet _wallet;
        private JsonSavingWrapper _savingWrapper;

        [Inject]
        private void Construct(JsonSavingWrapper savingSystemWrapper)
        {
            _savingWrapper = savingSystemWrapper;
        }

        private void Awake()
        {
            _inventory = GetComponent<Inventory>();
            _wallet = GetComponent<Wallet>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.TryGetComponent(out Buyer buyer))
            {
                SellCrops();
            }
        }

        private void SellCrops()
        {
            int receivedCoinsCount = 0;

            foreach (InventoryItem inventoryItem in _inventory.InventoryItems)
            {
                _wallet.ReceiveCoins(inventoryItem);
                _inventory.RemoveItem(inventoryItem);
            }
        }
    }
}
