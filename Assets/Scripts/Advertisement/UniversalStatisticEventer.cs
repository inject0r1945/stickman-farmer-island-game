using HCGame.Saving;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.Saving.Json;
using Zenject;

namespace HCGame.Advertisements
{
    public class UniversalStatisticEventer : MonoBehaviour, ISendingStatistic, IJsonSaveable
    {
        [SerializeField] private bool _isBlocked = false;

        private List<ISendingStatistic> _statisticEventers;
        private JsonSavingWrapper _savingWrapper;

        [Inject]
        private void Construct(JsonSavingWrapper savingSystemWrapper)
        {
            _savingWrapper = savingSystemWrapper;
        }

        private void Awake()
        {
            _statisticEventers = new List<ISendingStatistic>();

            foreach (ISendingStatistic statisticEventer in GetComponents<ISendingStatistic>())
            {
                MonoBehaviour statisticEventerMono = (MonoBehaviour)statisticEventer;

                if (statisticEventerMono == this || statisticEventerMono.enabled == false)
                    continue;

                _statisticEventers.Add(statisticEventer);
            }
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(_isBlocked);
        }

        public void RestoreFromJToken(JToken state)
        {
            _isBlocked = state.ToObject<bool>();
        }

        public void SendEvent()
        {
            if (_isBlocked || !gameObject.activeSelf)
                return;

            foreach (ISendingStatistic eventer in _statisticEventers)
            {
                eventer.SendEvent();
            }
        }

        public void Block()
        {
            _isBlocked = true;
            _savingWrapper.SendSignalToSave();
        }

        public void Unblock()
        {
            _isBlocked = false;
            _savingWrapper.SendSignalToSave();
        }
    }
}
