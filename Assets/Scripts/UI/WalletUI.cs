using HCGame.Attribute;
using HCGame.Shopping;
using HCGame.Utils.Static;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HCGame.UI
{
    public class WalletUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _coinsText;

        private Wallet _wallet;

        public UnityEvent<int> CoinsChanged;

        public UnityEvent<int> CoinsIncreased;

        private void Awake()
        {
            _wallet = Player.CurrentWallet;
        }

        private void OnEnable()
        {
            _wallet.CoinsChanged.AddListener(OnCoinsChanged);
            _wallet.CoinsIncreased.AddListener(OnCoinsIncreased);
        }

        private void OnDisable()
        {
            _wallet.CoinsChanged.RemoveListener(OnCoinsChanged);
            _wallet.CoinsIncreased.RemoveListener(OnCoinsIncreased);
        }

        private void OnCoinsChanged(int amount)
        {
            SetCoinsText(amount);
            CoinsChanged.Invoke(amount);
        }

        private void SetCoinsText(int amount)
        {
            _coinsText.text = amount.ToString();
        }

        private void OnCoinsIncreased(int amount)
        {
            SetCoinsText(amount);
            CoinsIncreased.Invoke(amount);
        }
    }
}
