using HCGame.Environment;
using HCGame.Inventories;
using HCGame.Saving;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utils.Saving.Json;
using Zenject;

namespace HCGame.Shopping
{
    public class Wallet : MonoBehaviour, IJsonSaveable
    {
        private int _coins;
        private JsonSavingWrapper _savingWrapper;

        public int Coins
        {
            get
            {
                return _coins;
            }

            private set 
            {
                if (value > _coins)
                    CoinsIncreased.Invoke(value);
                else if (value < _coins)
                    CoinsDecreased.Invoke(value);

                _coins = value;

                if (_coins < 0)
                    _coins = 0;

                CoinsChanged?.Invoke(_coins);
            }
        }

        [Inject]
        private void Construct(JsonSavingWrapper savingWrapper)
        {
            _savingWrapper = savingWrapper;
        }

        public UnityEvent<int> CoinsChanged;

        public UnityEvent<int> CoinsIncreased;

        public UnityEvent<int> CoinsDecreased;

        private void Start()
        {
            CoinsChanged.Invoke(Coins);
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(Coins);
        }

        public void RestoreFromJToken(JToken state)
        {
            _coins = state.ToObject<int>();
            CoinsChanged?.Invoke(_coins);
        }

        public void ReceiveCoins(InventoryItem inventoryItem)
        {
            if (inventoryItem.Amount == 0)
                return;

            Coins += inventoryItem.CropObject.Cost * inventoryItem.Amount;
            _savingWrapper.SendSignalToSave();
        }

        public int SpendCoins(int amount)
        {
            if (amount < 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(amount));
            }

            int spentCoins = (Coins < amount) ? Coins : amount;

            if (spentCoins == 0)
                return 0;

            Coins -= spentCoins;
            _savingWrapper.SendSignalToSave();

            return spentCoins;
        }

        public void AddCoins(int amount)
        {
            Coins += amount;
        }

        [NaughtyAttributes.Button]
        private void Add100Coins()
        {
            AddCoins(100);
        }
    }
}
