using HCGame.Control;
using HCGame.Utils.Static;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HCGame.Inventories;
using HCGame.Shopping;

namespace HCGame.Attribute
{
    [RequireComponent(typeof(PlayerTools))]
    [RequireComponent(typeof(Inventory))]
    [RequireComponent(typeof(Wallet))]
    public class Player : MonoBehaviour
    {
        private static Player _currentPlayer;
        private static Inventory _currentInventory;
        private static Wallet _currentWallet;

        public static Player CurrentPlayer 
        {
            get 
            {
                if (_currentPlayer == null)
                    _currentPlayer = StaticUtils.GetUniqueComponentOnScene<Player>();

                return _currentPlayer;
            } 
        }

        public static Inventory CurrentInventory
        {
            get
            {
                if (_currentInventory == null)
                    _currentInventory = CurrentPlayer.GetComponent<Inventory>();

                return _currentInventory;
            }
        }

        public static Wallet CurrentWallet
        {
            get
            {
                if (_currentWallet == null)
                    _currentWallet = CurrentPlayer.GetComponent<Wallet>();

                return _currentWallet;
            }
        }
    }
}
