using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.Events;

namespace HCGame.Environment
{
    public class CropTile : MonoBehaviour
    {
        [SerializeField] private Transform _cropParent;
        [SerializeField] private MeshRenderer _tileRenderer;
        [SerializeField] private Color _wateredColor;

        [Header("Parameters")]
        [SerializeField] private float _timeToChangeColor = 0.5f;
        [SerializeField] private int harvestAmount = 1;

        private CropTileState _state;
        private Crop _crop;
        private CropObject _cropObject;
        private Color _originalColor;

        public bool IsEmpty => _state == CropTileState.Empty;

        public bool IsSown => _state == CropTileState.Sown;

        public bool IsWatered => _state == CropTileState.Watered;

        public static event UnityAction<CropObject, int> CropHarvested;

        private void Start()
        {
            _state = CropTileState.Empty;
            _originalColor = _tileRenderer.material.color;
        }

        public void Sow(CropObject cropObject)
        {
            if (!IsEmpty)
                return;

            _state = CropTileState.Sown;
            _crop = cropObject.InstantiateCrop(transform.position, _cropParent);
            _cropObject = cropObject;
        }

        public void Water(CropObject cropObject)
        {
            if (!IsSown)
                return;

            _tileRenderer.material.DOColor(_wateredColor, _timeToChangeColor);
            _state = CropTileState.Watered;

            _crop.ScaleUp();
        }

        public void Harvest()
        {
            if (!IsWatered)
                return;

            _state = CropTileState.Empty;
            _crop.Destroy();
            _tileRenderer.material.DOColor(_originalColor, _timeToChangeColor);

            CropHarvested?.Invoke(_cropObject, harvestAmount);
        }

        public void Reset()
        {
            _state = CropTileState.Empty;
            _tileRenderer.material.color = _originalColor;
        }
    }
}
