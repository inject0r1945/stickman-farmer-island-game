using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HCGame.Advertisements
{
    public class UniversalAdvertisementShower : MonoBehaviour, IAdvertised
    {
        [SerializeField] private bool _isShowOnStart = false;

        private List<IAdvertised> _advertisiments;

        public event UnityAction AdvertisementShowSuccess;

        public event UnityAction AdvertisementShowFailed;

        public event UnityAction AdvertisementClosed;

        private void Awake()
        {
            _advertisiments = new List<IAdvertised>();

            foreach (IAdvertised advertisiment in GetComponentsInChildren<IAdvertised>())
            {
                MonoBehaviour advertisimentMono = (MonoBehaviour)advertisiment;

                if (advertisimentMono == this || advertisimentMono.enabled == false)
                    continue;

                _advertisiments.Add(advertisiment);
            }
        }

        private void OnEnable()
        {
            foreach (IAdvertised advertisiment in _advertisiments)
            {
                advertisiment.AdvertisementShowSuccess += OnAdvertisementShowSuccess;
                advertisiment.AdvertisementShowFailed += OnAdvertisementShowFailed;
                advertisiment.AdvertisementClosed += OnAdvertisementClosed;
            }
        }

        private void OnDisable()
        {
            foreach (IAdvertised advertisiment in _advertisiments)
            {
                advertisiment.AdvertisementShowSuccess -= OnAdvertisementShowSuccess;
                advertisiment.AdvertisementShowFailed -= OnAdvertisementShowFailed;
                advertisiment.AdvertisementClosed += OnAdvertisementClosed;
            }
        }

        private void Start()
        {
            if (_isShowOnStart && CanShowAdvertisement())
            {
                TryShowAdvertisement();
            }
        }

        public bool CanShowAdvertisement()
        {
            foreach (IAdvertised advertisiment in _advertisiments)
            {
                if (advertisiment.CanShowAdvertisement())
                    return true;
            }

            return false;
        }

        public bool TryShowAdvertisement()
        {
            bool isShowed = false;

            foreach (IAdvertised advertisiment in _advertisiments)
            {
                if (!advertisiment.CanShowAdvertisement())
                    continue;

                isShowed |= advertisiment.TryShowAdvertisement();
            }

            if (!isShowed)
                AdvertisementShowFailed?.Invoke();

            return isShowed;
        }

        public void ShowAdvertisement()
        {
            TryShowAdvertisement();
        }

        private void OnAdvertisementShowSuccess()
        {
            AdvertisementShowSuccess?.Invoke();
        }

        private void OnAdvertisementShowFailed()
        {
            AdvertisementShowFailed?.Invoke();
        }

        private void OnAdvertisementClosed()
        {
            AdvertisementClosed?.Invoke();
        }
    }
}
