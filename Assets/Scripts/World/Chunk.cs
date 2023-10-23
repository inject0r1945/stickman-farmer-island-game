using HCGame.Attribute;
using HCGame.Shopping;
using HCGame.Utils;
using HCGame.Stats;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Utils.Saving;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HCGame.Worlds
{
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(UniqueIdentificatorSetter))]
    public class Chunk : MonoBehaviour, IUniqueIdentifiable
    {
        [Header("Settings")]
        [SerializeField, Range(0, 50)] private int _priceLevel = 0;
        [SerializeField] private float _oneCoinChangeAnimationSeconds = 0.03f;
        [SerializeField] private float _maxCoinTextAnimationSeconds = 5f;
        [SerializeField] private float _chunkSize = 5;
        [SerializeField] private AudioSource _changePriceSound;
        [SerializeField] private ChunkCostStats _chunkCostStats;
        [SerializeField] private string _uniqueIdentifier = "";
        [SerializeField] private int _animationPriceThreshold = 50;

        [Header("Elements")]
        [SerializeField] private ChunkWalls _chunkWalls;
        [SerializeField] private GameObject _unlockedElements;
        [SerializeField] private GameObject _lockedElements;
        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private MeshFilter _groundMeshFilter;

        private int _currentPrice;
        private Wallet _wallet;
        private BoxCollider _boxCollider;
        private Coroutine _priceAnimateCoroutine;
        private Coroutine _unlockCoroutine;
        private bool _isAnimatedChangePrice;
        private int _textPricePreciseThreshold = 1000;
        private int _initialPrice;
        private bool _isChangedPriceByPlayer;

        public string UniqueIdentifier => _uniqueIdentifier;

        public int InitialPrice => _initialPrice;

        public int CurrentPrice
        {
            get
            {
                return _currentPrice;
            }

            private set
            {
                if (value < 0)
                    value = 0;

                if (gameObject.activeSelf && _isAnimatedChangePrice)
                {
                    if (_priceAnimateCoroutine != null)
                        StopCoroutine(_priceAnimateCoroutine);

                    IEnumerator priceAnimator = AnimateTextPriceChange(_currentPrice, value);
                    _priceAnimateCoroutine = StartCoroutine(priceAnimator);
                }
                else
                {
                    SetPriceText(value);
                }

                _currentPrice = value;

                if (gameObject.activeSelf)
                {
                    if (_unlockCoroutine != null)
                        StopCoroutine(_unlockCoroutine);

                    _unlockCoroutine = StartCoroutine(nameof(TryUnlock));
                }
            }
        }

        public bool IsUnlocked => CurrentPrice == 0;

        public bool IsChangedPriceByPlayer => _isChangedPriceByPlayer;

        public static event UnityAction<Chunk> PriceChanged;

        public static event UnityAction<Chunk> Unlocked;

        public UnityEvent CurrentChunkUnlocked;

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider>();
            _currentPrice = _chunkCostStats.GetLevelValue(_priceLevel);
            _initialPrice = _currentPrice;
        }

        private void OnDisable()
        {
            StopCoroutines();
        }

        private void Start()
        {
            SetPriceText(CurrentPrice);

            if (IsUnlocked)
                Unlock();
            else
                Lock();
        }

        private void OnTriggerEnter(Collider other)
        {  
            if (other.transform.TryGetComponent(out Player player))
            {
                _wallet = player.GetComponent<Wallet>();
                TakeMoneyToUnlock(_wallet);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, Vector3.one * _chunkSize);
        }

        public string GetUniqueIdentificator()
        {
            return _uniqueIdentifier;
        }

        public string GetUniqueIdentifivatorVariableName()
        {
            return nameof(_uniqueIdentifier);
        }

        public void Init()
        {
            Init(_initialPrice);
        }

        public void Init(int currentPrice)
        {
            _isAnimatedChangePrice = false;
            CurrentPrice = currentPrice;
        }

        public void Init(int currentPrice, bool isChangedPriceByPlayer)
        {
            Init(currentPrice);
            _isChangedPriceByPlayer = isChangedPriceByPlayer;
        }

        public void UpdateWalls(int wallsConfiguration)
        {
            _chunkWalls.Init(wallsConfiguration);
        }

        public void SetMesh(Mesh mesh)
        {
            if (_groundMeshFilter != null)
                _groundMeshFilter.mesh = mesh;
        }

        private IEnumerator AnimateTextPriceChange(int currentPrice, int nextPrice)
        {
            int reducePriceStep = (currentPrice <= nextPrice) ? 1 : -1;
            int reduceStepsCount = Mathf.Abs((currentPrice - nextPrice) / reducePriceStep);
            float oneStepWait = 0.007f;

            var waitForSeconds = new WaitForSeconds(oneStepWait);

            if (Mathf.Abs(currentPrice - nextPrice) > _animationPriceThreshold)
                currentPrice = nextPrice + _animationPriceThreshold * (-reducePriceStep);

            _changePriceSound.Play();

            while (currentPrice != nextPrice)
            {
                currentPrice += reducePriceStep;
                SetPriceText(currentPrice);

                yield return waitForSeconds;
            }

            _changePriceSound.Stop();
            _priceAnimateCoroutine = null;

            if (IsUnlocked)
            {
                CurrentChunkUnlocked?.Invoke();
                Unlocked?.Invoke(this);
            }  
        }

        private void SetPriceText(int price)
        {
            if (price > _textPricePreciseThreshold)
                _priceText.text = $"{(float)price / _textPricePreciseThreshold:N1}";
            else
                _priceText.text = price.ToString();
        }

        private IEnumerator TryUnlock()
        {
            while (_priceAnimateCoroutine != null)
                yield return null;

            if (IsUnlocked)
            {
                Unlock();
            }

            _unlockCoroutine = null;
        }

        private void Unlock()
        {
            _lockedElements.SetActive(false);
            _unlockedElements.SetActive(true);
            _boxCollider.enabled = false;
        }

        private void StopCoroutines()
        {
            if (_priceAnimateCoroutine != null)
            {
                StopCoroutine(_priceAnimateCoroutine);
                _priceAnimateCoroutine = null;
            }

            if (_unlockCoroutine != null)
            {
                StopCoroutine(_unlockCoroutine);
                _unlockCoroutine = null;
            }
        }

        private void Lock()
        {
            _unlockedElements.SetActive(false);
            _lockedElements.SetActive(true);
            _boxCollider.enabled = true;
        }

        private bool TakeMoneyToUnlock(Wallet wallet)
        {
            if (CurrentPrice > 0)
            {
                int receivedCoins = wallet.SpendCoins(CurrentPrice);

                if (receivedCoins > 0)
                {
                    _isAnimatedChangePrice = true;
                    CurrentPrice -= receivedCoins;
                    _isChangedPriceByPlayer = true;

                    PriceChanged?.Invoke(this);
                }
            }

            return IsUnlocked;
        } 
    }
}
