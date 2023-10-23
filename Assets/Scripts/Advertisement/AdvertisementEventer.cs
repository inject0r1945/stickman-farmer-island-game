using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HCGame.Advertisements
{
    [RequireComponent(typeof(UniversalAdvertisementShower))]
    public class AdvertisementEventer : MonoBehaviour
    {
        private UniversalAdvertisementShower _advertisementShower;

        public UnityEvent AdvertisementClosed;

        public UnityEvent AdvertisementShowSuccess;

        public UnityEvent AdvertisementShowFailed;

        private void Awake()
        {
            _advertisementShower = GetComponent<UniversalAdvertisementShower>();
        }

        private void OnEnable()
        {
            _advertisementShower.AdvertisementShowSuccess += OnAdvertisementShowSuccess;
            _advertisementShower.AdvertisementShowFailed += OnAdvertisementShowFailed;
            _advertisementShower.AdvertisementClosed += OnAdvertisementClosed;
        }

        private void OnDisable()
        {
            _advertisementShower.AdvertisementShowSuccess -= OnAdvertisementShowSuccess;
            _advertisementShower.AdvertisementShowFailed -= OnAdvertisementShowFailed;
            _advertisementShower.AdvertisementClosed -= OnAdvertisementClosed;
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
