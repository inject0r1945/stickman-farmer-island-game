using HCGame.Attribute;
using HCGame.Control;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HCGame.UI
{
    public class ToolItemsContainerUI : MonoBehaviour
    {
        private ToolItemUI[] _toolItemsUI;
        private PlayerTools _playerTools;

        private void Awake()
        {
            _playerTools = Player.CurrentPlayer.GetComponent<PlayerTools>();
            _toolItemsUI = GetComponentsInChildren<ToolItemUI>();
        }

        private void OnEnable()
        {
            _playerTools.ToolChanged += OnToolChanged;
        }

        private void OnDisable()
        {
            _playerTools.ToolChanged -= OnToolChanged;
        }

        private void OnToolChanged(ToolType activeToolType)
        {
            Redraw(activeToolType);
        }

        private void Redraw(ToolType activeToolType)
        {
            foreach (ToolItemUI toolItemUi in _toolItemsUI)
            {
                if (activeToolType == toolItemUi.ToolType)
                    toolItemUi.Select();
                else
                    toolItemUi.Unselect();
            }
        }
    }
}
