using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
using AppodealStack.Monetization.Api;
using AppodealStack.Monetization.Common;
#endif
using UnityEngine.Events;
using System;

namespace HCGame.Advertisements
{
    public class AppodealRewardAdvertisiment : MonoBehaviour, IAdvertised
    {
        [SerializeField] private FirebaseEventer _firebaseEventer;

#if UNITY_ANDROID
        private int _advertisimentType = AppodealAdType.RewardedVideo;
#endif

        public event UnityAction AdvertisementShowSuccess;

        public event UnityAction AdvertisementShowFailed;

        public event UnityAction AdvertisementClosed;

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
            Debug.Log("Запрошена реклама Appodeal Rewarded");

            if (!CanShowAdvertisement())
            {
                AdvertisementShowFailed?.Invoke();
                return false;
            }

            AddSubscriptions();
            AudioListener.pause = true;
            Appodeal.Show(AppodealShowStyle.RewardedVideo);

            return true;
#else
            return false;
#endif
        }

        public void ShowAdvertisement()
        {
#if UNITY_ANDROID
            TryShowAdvertisement();
#endif
        }

#if UNITY_ANDROID
        private void AddSubscriptions()
        {
            AppodealCallbacks.RewardedVideo.OnFailedToLoad += OnRewardedVideoFailedToLoad;
            AppodealCallbacks.RewardedVideo.OnShowFailed += OnRewardedVideoShowFailed;
            AppodealCallbacks.RewardedVideo.OnClosed += OnRewardedVideoClosed;
            AppodealCallbacks.RewardedVideo.OnFinished += OnRewardedVideoFinished;
            AppodealCallbacks.RewardedVideo.OnClicked += OnRewardedVideoClicked;
        }

        private void OnRewardedVideoFailedToLoad(object sender, EventArgs eventArgs)
        {
            OnFailedAdvertisementShow();
        }

        private void OnFailedAdvertisementShow()
        {
            AdvertisementShowFailed?.Invoke();
            DeleteSubscriptions();
            AudioListener.pause = false;
        }

        private void OnRewardedVideoShowFailed(object sender, EventArgs eventArgs)
        {
            OnFailedAdvertisementShow();
        }

        private void OnRewardedVideoClosed(object sender, RewardedVideoClosedEventArgs eventArgs)
        {
            OnSuccessAdvertisementShow();
            AdvertisementClosed?.Invoke();
        }

        private void OnSuccessAdvertisementShow()
        {
            AdvertisementShowSuccess?.Invoke();
            _firebaseEventer?.SendRewardedAdvertisementImpressionEvent();
            DeleteSubscriptions();
            AudioListener.pause = false;
        }

        private void OnRewardedVideoClicked(object sender, EventArgs eventArgs)
        {
            OnSuccessAdvertisementShow();
            _firebaseEventer?.SendRewardedAdvertisementClickEvent();
        }

        private void OnRewardedVideoFinished(object sender, RewardedVideoFinishedEventArgs eventArgs)
        {
            OnSuccessAdvertisementShow();
        }

        private void DeleteSubscriptions()
        {
            AppodealCallbacks.RewardedVideo.OnFailedToLoad -= OnRewardedVideoFailedToLoad;
            AppodealCallbacks.RewardedVideo.OnShowFailed -= OnRewardedVideoShowFailed;
            AppodealCallbacks.RewardedVideo.OnClosed -= OnRewardedVideoClosed;
            AppodealCallbacks.RewardedVideo.OnFinished -= OnRewardedVideoFinished;
            AppodealCallbacks.RewardedVideo.OnClicked -= OnRewardedVideoClicked;
        }
#endif
    }
}
