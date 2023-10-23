using HCGame.Tutorials;
using Newtonsoft.Json.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace HCGame.Saving
{
    public class YandexSaveDataReceiver : MonoBehaviour
    {
        public UnityEvent OnAwake;

        public UnityEvent SaveLoaded;

#if UNITY_WEBGL
        private JsonSavingWrapper _savingWrapper;

        [Inject]
        private void Construct(JsonSavingWrapper savingWrapper)
        {
            _savingWrapper = savingWrapper;
        }

        private void Awake()
        {
            OnAwake.Invoke();
        }

        public void Receive(string stateJson)
        {
            JObject state = JObject.Parse(stateJson);
            _savingWrapper.Load(state);
            SaveLoaded.Invoke();
        }
#endif
    }
}