using UnityEngine;
#if UNITY_ANDROID
using Firebase.Analytics;
#endif
using System;
using Zenject;
using System.Collections;

namespace HCGame.Advertisements
{
    public class FirebaseStaticEventer : MonoBehaviour, ISendingStatistic
    {
        [SerializeField] private StatisticEvent _event;
        [SerializeField] private bool _isSendEventOnStart = false;

#if UNITY_ANDROID
        private FirebaseInitializer _firebaseInitializer;
        private Coroutine _eventCoroutine;

        [Inject]
        private void Construct(FirebaseInitializer firebaseInitializer)
        {
            _firebaseInitializer = firebaseInitializer;
        }

        void Start()
        {
            if (_isSendEventOnStart)
                SendEvent();
        }
#endif

        public void SendEvent()
        {
#if UNITY_ANDROID
            if (_eventCoroutine != null)
                StopCoroutine(_eventCoroutine);

            _eventCoroutine = StartCoroutine(SendEventCoroutine());
#endif
        }

#if UNITY_ANDROID
        private IEnumerator SendEventCoroutine()
        {
            while (!_firebaseInitializer.IsReady)
                yield return null;

            string eventName = Enum.GetName(typeof(StatisticEvent), _event);
            FirebaseAnalytics.LogEvent(eventName);
            Debug.Log($"Sended Firebase event: {_event}");
        }
#endif
    }
}
