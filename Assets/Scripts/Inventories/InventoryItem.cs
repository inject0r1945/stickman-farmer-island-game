using HCGame.Environment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HCGame.Inventories
{
    public class InventoryItem
    {
        private CropObject _cropObject;
        private int _amount;

        public CropObject CropObject => _cropObject;

        public int Amount => _amount;

        public InventoryItem(CropObject cropObject)
        {
            _cropObject = cropObject;
        }

        public InventoryItem(CropObject cropObject, int amount) : this(cropObject)
        {
            _amount = amount;
        }

        public void IncreaseAmount(int count)
        {
            _amount += count;

            if (_amount < 0)
                _amount = 0;
        }
    }
}