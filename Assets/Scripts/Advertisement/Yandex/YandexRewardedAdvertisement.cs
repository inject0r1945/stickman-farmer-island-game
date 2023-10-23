using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

namespace HCGame.Advertisements
{
    public class YandexRewardedAdvertisement : MonoBehaviour, IAdvertised
    {
        [SerializeField] private bool _isLogToWebConsole = false;

#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void ShowRewardedAdvertisementSDK();

        [DllImport("__Internal")]
        private static extern void ShowConsoleMessage(string message);
#endif

        public event UnityAction AdvertisementShowSuccess;

        public event UnityAction AdvertisementShowFailed;

        public event UnityAction AdvertisementClosed;

#if UNITY_WEBGL
        private void OnEnable()
        {
            YandexAdvertisimentsHandler.RewardedAdvertisimentClose += OnCloseAdvertisement;
            YandexAdvertisimentsHandler.RewardedAdvertisimentComplete += OnRewardedAdvertisement;
            YandexAdvertisimentsHandler.RewardedAdvertisimentError += OnErrorAdvertisement;
        }

        private void OnDisable()
        {
            YandexAdvertisimentsHandler.RewardedAdvertisimentClose -= OnCloseAdvertisement;
            YandexAdvertisimentsHandler.RewardedAdvertisimentComplete -= OnRewardedAdvertisement;
            YandexAdvertisimentsHandler.RewardedAdvertisimentError -= OnErrorAdvertisement;
        }
#endif

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
            Debug.Log("Запрошена реклама Yandex Rewarded");

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
            ShowRewardedAdvertisementSDK();
            DisableAudioListener();

            if (_isLogToWebConsole)
                ShowConsoleMessage("Отправлен запрос в SDK для показа Rewarded Ad");
#endif
        }

#if UNITY_WEBGL
        private void OnCloseAdvertisement()
        {
            if (_isLogToWebConsole)
                ShowConsoleMessage("Получен обратный вызов Rewarded Ad при закрытии");

            AdvertisementClosed?.Invoke();

            EnableAudioListener();
        }

        private void OnErrorAdvertisement()
        {
            if (_isLogToWebConsole)
                ShowConsoleMessage("Получен обратный вызов Rewarded Ad при ошибке");

            EnableAudioListener();
            AdvertisementShowFailed.Invoke();
        }

        private void OnRewardedAdvertisement()
        {
            if (_isLogToWebConsole)
                ShowConsoleMessage("Получен обратный вызов Rewarded Ad для получения награды");

            AdvertisementShowSuccess.Invoke();
        }

        private void DisableAudioListener()
        {
            AudioListener.pause = true;
        }

        private void EnableAudioListener()
        {
            AudioListener.pause = false;
        }
#endif
    }
}
