using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utils.Saving.Json;
using TMPro;
using DG.Tweening;
using HCGame.Attribute;
using Zenject;
using HCGame.Saving;

namespace HCGame.Environment
{
    public class CropField : MonoBehaviour, IJsonSaveable
    {
        [Header("Elements")]
        [SerializeField] private Transform _tilesParent;
        [SerializeField] private Transform _blockObjectsParent;
        [SerializeField] private TMP_Text _blockTimerText;
        [SerializeField] private MeshRenderer _cropIconRenderer;
        [SerializeField] private Canvas _blockCanvas;

        [Header("Settings")]
        [SerializeField] private CropObject _cropObject;
        [SerializeField] private int _harvestCountToBlock = 2;
        [SerializeField] private float _defaultBlockTime = 180f;
        [SerializeField] private float _saveTimePeriodInBlock = 30f;

        private int _harvestCounter;
        private float _blockTimer;
        private CropTile[] _cropTiles;
        private Coroutine _unsetBlockCoroutine;
        private Tween _iconFadeTween;
        private string _isBlockedStateSaveName = "IsBlocked";
        private string _blockTimerSaveName = "BlockTimer";
        private string _harvestCounterSaveName = "HarvestCounter";
        private JsonSavingWrapper _savingWrapper;

        public bool IsBlocked { get; private set; }

        public float BlockTimer => _blockTimer;

        public bool IsAllTilesEmpty
        {
            get
            {
                foreach (CropTile cropTile in _cropTiles)
                {
                    if (!cropTile.IsEmpty)
                        return false;
                }

                return true;
            }
        }

        public bool IsAllTilesSown
        {
            get
            {
                foreach (CropTile cropTile in _cropTiles)
                {
                    if (!cropTile.IsSown)
                        return false;
                }

                return true;
            }
        }

        public bool IsAllTilesWatered
        {
            get
            {
                foreach (CropTile cropTile in _cropTiles)
                {
                    if (!cropTile.IsWatered)
                        return false;
                }

                return true;
            }
        }

        public bool HasEmptyTiles
        {
            get
            {
                foreach (CropTile cropTile in _cropTiles)
                {
                    if (cropTile.IsEmpty)
                        return true;
                }

                return false;
            }
        }

        public bool HasSownTiles
        {
            get
            {
                foreach (CropTile cropTile in _cropTiles)
                {
                    if (cropTile.IsSown)
                        return true;
                }

                return false;
            }
        }

        public bool HasWateredTiles
        {
            get
            {
                foreach (CropTile cropTile in _cropTiles)
                {
                    if (cropTile.IsWatered)
                        return true;
                }

                return false;
            }
        }

        public static event UnityAction<CropField> FullySown;

        public static event UnityAction<CropField> FullyWatered;

        public static event UnityAction<CropField> FullyHarvested;

        public static event UnityAction<CropField> Unblocked;

        [Inject]
        private void Construct(JsonSavingWrapper savingSystemWrapper)
        {
            _savingWrapper = savingSystemWrapper;
        }

        private void Awake()
        {
            _cropIconRenderer.material = _cropObject.CropMaterial;
        }

        private void Start()
        {
            StoreTiles();
            FadeInCropIcon();
            _blockCanvas.gameObject.SetActive(false);
        }

        private void OnTriggerStay(Collider other)
        {
            OnTriggerEnter(other);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsBlocked)
               return;

            if (!_blockCanvas.gameObject.activeSelf 
                && other.gameObject.TryGetComponent(out Player player))
            {
                _blockCanvas.gameObject.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_blockCanvas.gameObject.activeSelf)
                _blockCanvas.gameObject.SetActive(false);
        }

        public JToken CaptureAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDictionary = state;

            stateDictionary.Add(_isBlockedStateSaveName, IsBlocked);
            stateDictionary.Add(_blockTimerSaveName, _blockTimer);
            stateDictionary.Add(_harvestCounterSaveName, _harvestCounter);

            return state;
        }

        public void RestoreFromJToken(JToken state)
        {
            IDictionary<string, JToken> stateDictionary = (IDictionary<string, JToken>)state;

            if (stateDictionary.TryGetValue(_isBlockedStateSaveName, out JToken isBlockedToken))
            {
                IsBlocked = isBlockedToken.ToObject<bool>();
            }

            if (stateDictionary.TryGetValue(_blockTimerSaveName, out JToken blockTimerToken))
            {
                _blockTimer = blockTimerToken.ToObject<float>();
            }

            if (stateDictionary.TryGetValue(_harvestCounterSaveName, out JToken harvestCounterToken))
            {
                _harvestCounter = harvestCounterToken.ToObject<int>();
            }

            if (IsBlocked)
                Block(_blockTimer);
        }

        public void SeedCollidedCallback(Vector3[] seedsPositions)
        {
            if (IsBlocked ||!HasEmptyTiles)
                return;

            FadeOutCropIcon();

            foreach (Vector3 seedPosition in seedsPositions)
            {
                CropTile closestCropTile = GetClosestCropTile(seedPosition);

                if (closestCropTile == null)
                    continue;

                if (!closestCropTile.IsEmpty)
                    continue;

                Sow(closestCropTile);
            }

            if (IsAllTilesSown)
                FullySown?.Invoke(this);
        }

        public void WaterCollidedCallback(Vector3[] waterPositions)
        {
            if (IsBlocked || !HasSownTiles)
                return;

            foreach (Vector3 waterPosition in waterPositions)
            {
                CropTile closestCropTile = GetClosestCropTile(waterPosition);

                if (closestCropTile == null)
                    continue;

                if (!closestCropTile.IsSown)
                    continue;

                Water(closestCropTile);
            }

            if (IsAllTilesWatered)
                FullyWatered?.Invoke(this);
        }

        public void Harvest(Transform harvestSphere)
        {
            float sphereRadius = harvestSphere.localScale.x;

            foreach (CropTile cropTile in _cropTiles)
            {
                if (cropTile.IsEmpty)
                    continue;

                float distanceToCropTile = Vector3.Distance(harvestSphere.position, cropTile.transform.position);

                if (distanceToCropTile > sphereRadius)
                    continue;

                cropTile.Harvest();

                if (IsAllTilesEmpty)
                {
                    _harvestCounter++;
                    FadeInCropIcon();
                    FullyHarvested?.Invoke(this);
                }

                if (_harvestCounter >= _harvestCountToBlock)
                {
                    Block();
                }
            }
        }

        public void Reset()
        {
            foreach (CropTile cropTile in _cropTiles)
            {
                cropTile.Reset();
            }
        }

        public void Unblock()
        {
            if (_unsetBlockCoroutine != null)
                StopCoroutine(_unsetBlockCoroutine);

            _blockTimer = 0;
            IsBlocked = false;
            _harvestCounter = 0;
            _blockObjectsParent.gameObject.SetActive(false);
            _blockCanvas.gameObject.SetActive(false);

            _savingWrapper.SendSignalToSave();
        }

        private void StoreTiles()
        {
            _cropTiles = _tilesParent.GetComponentsInChildren<CropTile>();
        }

        private void FadeInCropIcon()
        {
            _cropIconRenderer.enabled = true;
        }

        private void FadeOutCropIcon()
        {
            _cropIconRenderer.enabled = false;
        }

        private CropTile GetClosestCropTile(Vector3 seedPosition)
        {
            float minDistance = Mathf.Infinity;
            CropTile closestCropTile = null;

            foreach (CropTile currentCropTile in _cropTiles)
            {
                float tileSeedDistance = Vector3.Distance(currentCropTile.transform.position, seedPosition);

                if (tileSeedDistance < minDistance)
                {
                    minDistance = tileSeedDistance;
                    closestCropTile = currentCropTile;
                }  
            }

            return closestCropTile;
        }

        private void Sow(CropTile closestCropTile)
        {
            closestCropTile.Sow(_cropObject);
        }

        private void Water(CropTile closestCropTile)
        {
            closestCropTile.Water(_cropObject);
        }

        private void Block()
        {
            Block(_defaultBlockTime);
            _savingWrapper.SendSignalToSave();
        }

        private void Block(float blockTime)
        {
            IsBlocked = true;
            _blockTimer = blockTime;
            _blockObjectsParent.gameObject.SetActive(true);

            _unsetBlockCoroutine = StartCoroutine(nameof(UnblockTimerCoroutine));
        }

        private IEnumerator UnblockTimerCoroutine()
        {
            TimeSpan blockTimeSpan;
            float saveTimer = 0f;

            while (_blockTimer > 0)
            {
                blockTimeSpan = TimeSpan.FromSeconds(_blockTimer);
                _blockTimerText.text = blockTimeSpan.ToString(@"hh\:mm\:ss");

                _blockTimer -= Time.deltaTime;
                saveTimer += Time.deltaTime;

                if (saveTimer > _saveTimePeriodInBlock)
                {
                    _savingWrapper.SendSignalToSave();
                    saveTimer = 0;
                }

                yield return null;
            }

            Unblock();
        }

        [NaughtyAttributes.Button]
        private void SowInstantly()
        {
            FadeOutCropIcon();

            foreach (CropTile currentCropTile in _cropTiles)
            {
                currentCropTile.Sow(_cropObject);
            }
        }

        [NaughtyAttributes.Button]
        private void WaterInstantly()
        {
            foreach (CropTile currentCropTile in _cropTiles)
            {
                currentCropTile.Water(_cropObject);
            }
        }
    }
}