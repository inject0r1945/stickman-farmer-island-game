using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HCGame.Advertisements
{
    public class YandexAdvertisimentsHandler : MonoBehaviour
    {
        static public event UnityAction RewardedAdvertisimentComplete;

        static public event UnityAction RewardedAdvertisimentError;

        static public event UnityAction RewardedAdvertisimentClose;

        private void OnCloseAdvertisement()
        {
            RewardedAdvertisimentClose?.Invoke();
        }

        private void OnErrorAdvertisement()
        {
            RewardedAdvertisimentError?.Invoke();
        }

        private void OnRewardedAdvertisement()
        {
            RewardedAdvertisimentComplete?.Invoke();
        }
    }
}
