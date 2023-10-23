using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HCGame.Environment;
using HCGame.Inventories;
using HCGame.Utils.Static;

namespace HCGame.UI
{
    public class CropContainerUI : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _amountText;
        [SerializeField] private CropType _cropType;

        public CropType CropType => _cropType;

        public void Init(CropObject cropObject, int amount)
        {
            _icon.sprite = cropObject.Icon;
            _amountText.text = amount.ToString();
            _cropType = cropObject.CropType;
        }
    }
}