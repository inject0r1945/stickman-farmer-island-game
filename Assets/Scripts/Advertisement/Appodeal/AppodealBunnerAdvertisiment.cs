using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_ANDROID
using AppodealStack.Monetization.Api;
using AppodealStack.Monetization.Common;
#endif
using System;

namespace HCGame.Advertisements
{
    public class AppodealBunnerAdvertisiment : MonoBehaviour, IAdvertised
    {
        [SerializeField] private FirebaseEventer _firebaseEventer;

        public event UnityAction AdvertisementShowSuccess;

        public event UnityAction AdvertisementShowFailed;

        public event UnityAction AdvertisementClosed;

#if UNITY_ANDROID
        private int _advertisimentPosition = AppodealShowStyle.BannerBottom;

        private void OnEnable()
        {
            AppodealCallbacks.Banner.OnClicked += OnBannerClicked;
            AppodealCallbacks.Banner.OnFailedToLoad += OnBannerFailed;
            AppodealCallbacks.Banner.OnShowFailed += OnBannerFailed;
        }

        private void OnDisable()
        {
            AppodealCallbacks.Banner.OnClicked -= OnBannerClicked;
            AppodealCallbacks.Banner.OnFailedToLoad -= OnBannerFailed;
            AppodealCallbacks.Banner.OnShowFailed -= OnBannerFailed;
        }

        private void OnDestroy()
        {
            Appodeal.Destroy(AppodealAdType.Banner);
        }

        private void Start()
        {
            Appodeal.Show(_advertisimentPosition);
        }
#endif

        public bool CanShowAdvertisement()
        {
#if UNITY_ANDROID
            return true;
#else
            return false;
#endif
        }

        public bool TryShowAdvertisement()
        {
#if UNITY_ANDROID
            Appodeal.Show(_advertisimentPosition);
            return true;
#else
            return false;
#endif
        }

        private void OnBannerClicked(object sender, EventArgs eventsArgs)
        {
#if UNITY_ANDROID
            _firebaseEventer?.SendBunnerAdvertisementClickEvent();
#endif
        }

        private void OnBannerFailed(object sender, EventArgs eventsArgs)
        {
            AdvertisementShowFailed.Invoke();
        }
    }
}
