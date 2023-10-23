using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
using AppodealStack.Monetization.Api;
#endif
using System;

namespace HCGame.Advertisements
{
    public class AppodealFirebaseEventer : MonoBehaviour, ISendingStatistic
    {
        [SerializeField] private StatisticEvent _event;
        [SerializeField] private bool _isSendEventOnStart = false;

        private void Start()
        {
            if (_isSendEventOnStart)
                SendEvent();
        }

        public void SendEvent()
        {
#if UNITY_ANDROID
            string eventName = Enum.GetName(typeof(StatisticEvent), _event);
            Appodeal.LogEvent(eventName);
            Debug.Log($"Отправлен event firebase: {eventName}");
#endif
        }
    }
}
