using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

namespace HCGame.Advertisements
{
    public class YandexInterstitialAdvertisement : MonoBehaviour, IAdvertised
    {
#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void ShowInterstitialAdvertisementSDK();
#endif

        public event UnityAction AdvertisementShowSuccess;

        public event UnityAction AdvertisementShowFailed;

        public event UnityAction AdvertisementClosed;

        public bool CanShowAdvertisement()
        {
#if UNITY_WEBGL
            return true;
#else
            return false;
#endif
        }

        public bool TryShowAdvertisement()
        {
            Debug.Log("Запрошена реклама Yandex Interstitial");

            if (!CanShowAdvertisement())
            {
                AdvertisementShowFailed?.Invoke();
                return false;
            }

            ShowAdvertisement();

            return true;
        }

        public void ShowAdvertisement()
        {
#if UNITY_WEBGL
            ShowInterstitialAdvertisementSDK();
            AudioListener.pause = true;
#endif
        }

        private void OnClose()
        {
            AdvertisementShowSuccess?.Invoke();
            AdvertisementClosed?.Invoke();
            AudioListener.pause = false;
    }

        private void OnError()
        {
            AdvertisementShowFailed?.Invoke();
            AudioListener.pause = false;
        }
    }
}
