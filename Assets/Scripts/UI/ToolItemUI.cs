using HCGame.Attribute;
using HCGame.Control;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HCGame.UI
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public class ToolItemUI : MonoBehaviour
    {
        [SerializeField] private ToolType _toolType;
        [SerializeField] private Color _activeToolColor = Color.blue;
        [SerializeField] private Color _notActiveToolColor = Color.white;

        private PlayerTools _playerTools;
        private Button _button;
        private Image _image;

        public ToolType ToolType => _toolType;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _image = GetComponent<Image>();
            _playerTools = Player.CurrentPlayer.GetComponent<PlayerTools>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClick);
        }

        public void Select()
        {
            _image.color = _activeToolColor;
        }

        public void Unselect()
        {
            _image.color = _notActiveToolColor;
        }

        private void OnButtonClick()
        {
            _playerTools.SelectTool(_toolType);
        }
    }
}
