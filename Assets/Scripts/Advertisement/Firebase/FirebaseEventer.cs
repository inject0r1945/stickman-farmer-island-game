#if UNITY_ANDROID
using Firebase.Analytics;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace HCGame.Advertisements
{
    public class FirebaseEventer : MonoBehaviour
    {
#if UNITY_ANDROID
        private FirebaseInitializer _firebaseInitializer;
        private readonly string _adverstitionParameterName = "ad_type";
        private readonly string _adverstionRewardedParameterValue = "rewarded_ad";
        private readonly string _adverstionBannerParameterValue = "banner_ad";
        private readonly string _adverstionInterstitialParameterValue = "interstitial_ad";
        private readonly string _advertisimentClickParameterName = "advertisiment_clicked";
        private Parameter _rewardedAdvertisementParameter;
        private Parameter _bannerAdvertisementParameter;
        private Parameter _interstitialAdvertisementParameter;

        [Inject]
        private void Construct(FirebaseInitializer firebaseInitializer)
        {
            _firebaseInitializer = firebaseInitializer;
        }

        private void Awake()
        {
            _rewardedAdvertisementParameter = new Parameter(_adverstitionParameterName,
                _adverstionRewardedParameterValue);

            _bannerAdvertisementParameter = new Parameter(_adverstitionParameterName,
                _adverstionBannerParameterValue);

            _interstitialAdvertisementParameter = new Parameter(_adverstitionParameterName,
                _adverstionInterstitialParameterValue);
        }
#endif

        public void SendRewardedAdvertisementClickEvent()
        {
#if UNITY_ANDROID
            SendEvent(_advertisimentClickParameterName, _rewardedAdvertisementParameter);
#endif
        }

        public void SendBunnerAdvertisementClickEvent()
        {
#if UNITY_ANDROID
            SendEvent(_advertisimentClickParameterName, _bannerAdvertisementParameter);
#endif
        }

        public void SendInterstitialAdvertisementClickEvent()
        {
#if UNITY_ANDROID
            SendEvent(_advertisimentClickParameterName, _interstitialAdvertisementParameter);
#endif
        }

        public void SendRewardedAdvertisementImpressionEvent()
        {
#if UNITY_ANDROID
            SendAdvertisementImpressionEvent(_rewardedAdvertisementParameter);
#endif
        }

        public void SendBunnerAdvertisementImpressionEvent()
        {
#if UNITY_ANDROID
            SendAdvertisementImpressionEvent(_bannerAdvertisementParameter);
#endif
        }

        public void SendInterstitialAdvertisementImpressionEvent()
        {
#if UNITY_ANDROID
            SendAdvertisementImpressionEvent(_interstitialAdvertisementParameter);
#endif
        }

#if UNITY_ANDROID
        private void SendAdvertisementImpressionEvent(Parameter parameter)
        {
            SendEvent(FirebaseAnalytics.EventAdImpression, parameter);
        }

        private void SendEvent(string eventName, Parameter parameter)
        {
            if (!_firebaseInitializer.IsReady)
                return;

            FirebaseAnalytics.LogEvent(eventName, parameter);
            Debug.Log($"Sended Firebase event: {eventName}");
        }
#endif
    }
}