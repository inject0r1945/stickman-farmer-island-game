using UnityEngine;
using UnityEngine.Events;
using System;
#if UNITY_ANDROID
using AppodealStack.Monetization.Api;
using AppodealStack.Monetization.Common;
#endif

namespace HCGame.Advertisements
{
    public class AppodealInterstitialAdvertisiment : MonoBehaviour, IAdvertised
    {
        [SerializeField] private FirebaseEventer _firebaseEventer;

#if UNITY_ANDROID
        private int _advertisimentType = AppodealAdType.Interstitial;
#endif

        public event UnityAction AdvertisementShowSuccess;

        public event UnityAction AdvertisementShowFailed;

        public event UnityAction AdvertisementClosed;

#if UNITY_ANDROID
        private void OnEnable()
        {
            AppodealCallbacks.Interstitial.OnShown += OnAdvertisimentShown;
            AppodealCallbacks.Interstitial.OnClicked += OnAdvertisimentClicked;
            AppodealCallbacks.Interstitial.OnClosed += OnAdvertisimentClose;
            AppodealCallbacks.Interstitial.OnShowFailed += OnInterstitialShowFailed;
            AppodealCallbacks.Interstitial.OnExpired += OnInterstitialExpired;
            AppodealCallbacks.Interstitial.OnFailedToLoad += OnInterstitialFailedToLoad;
        }

        private void OnDisable()
        {
            AppodealCallbacks.Interstitial.OnShown -= OnAdvertisimentShown;
            AppodealCallbacks.Interstitial.OnClicked -= OnAdvertisimentClicked;
            AppodealCallbacks.Interstitial.OnClosed -= OnAdvertisimentClose;
            AppodealCallbacks.Interstitial.OnShowFailed -= OnInterstitialShowFailed;
            AppodealCallbacks.Interstitial.OnExpired -= OnInterstitialExpired;
            AppodealCallbacks.Interstitial.OnFailedToLoad -= OnInterstitialFailedToLoad;
        }
#endif

        public bool CanShowAdvertisement()
        {
#if UNITY_ANDROID
            return Appodeal.IsLoaded(_advertisimentType) && Appodeal.CanShow(_advertisimentType)
                && !Appodeal.IsPrecache(_advertisimentType);
#else
            return false;
#endif
        }

        public bool TryShowAdvertisement()
        {
#if UNITY_ANDROID
            Debug.Log("Запрошена реклама Appodeal Interstitial");

            if (!CanShowAdvertisement())
            {
                return false;
            }
                
            AudioListener.pause = true;
            Show();

            return true;
#else
            return false;
#endif
        }

        public void ShowAdvertisement()
        {
            TryShowAdvertisement();
        }

#if UNITY_ANDROID
        private void Show()
        {
            Appodeal.Show(AppodealShowStyle.Interstitial);
        }

        private void OnAdvertisimentShown(object sender, EventArgs eventArgs)
        {
            _firebaseEventer?.SendInterstitialAdvertisementImpressionEvent();
            AudioListener.pause = true;
        }

        private void OnAdvertisimentClicked(object sender, EventArgs eventArgs)
        {
            _firebaseEventer?.SendInterstitialAdvertisementClickEvent();
        }

        private void OnAdvertisimentClose(object sender, EventArgs eventArgs)
        {
            AdvertisementShowSuccess?.Invoke();
            AdvertisementClosed?.Invoke();
            OnAdvertisimentEndShow();
        }

        private void OnAdvertisimentEndShow()
        {
            AudioListener.pause = false;
        }

        private void OnInterstitialExpired(object sender, EventArgs eventArgs)
        {
            AdvertisementClosed?.Invoke();
        }

        private void OnInterstitialShowFailed(object sender, EventArgs eventArgs)
        {
            AdvertisementShowFailed?.Invoke();
            OnAdvertisimentEndShow();
            AdvertisementClosed?.Invoke();
        }

        private void OnInterstitialFailedToLoad(object sender, EventArgs eventArgs)
        {
            AdvertisementShowFailed?.Invoke();
            OnAdvertisimentEndShow();
            AdvertisementClosed?.Invoke();
        }
#endif
    }
}
