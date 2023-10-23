using UnityEngine;
using UnityEngine.Events;

namespace HCGame.Control
{
    public class PlayerTools : MonoBehaviour
    {
        private ToolType _activeTool;

        public bool IsCanSow => _activeTool == ToolType.Sow;

        public bool IsCanWater => _activeTool == ToolType.Water;

        public bool IsCanHarvest => _activeTool == ToolType.Harvest;

        public event UnityAction<ToolType> ToolChanged;

        private void Start()
        {
            SelectTool(ToolType.None);
        }

        public void SelectTool(ToolType toolType)
        {
            _activeTool = toolType;
            ToolChanged?.Invoke(_activeTool);
        }
    }
}