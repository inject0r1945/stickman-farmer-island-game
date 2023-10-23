using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Utils.Saving.Json
{
    [CreateAssetMenu(menuName = "SavingStrategies/YandexCloud", fileName = "YandexCloudStrategy")]
    public class YandexCloudSavingStrategy : SavingStrategy
    {
#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void SendSaveToYandexCloud(string state);

        [DllImport("__Internal")]
        private static extern string SendRequestToLoadSaveFromYandexCloud();

        [DllImport("__Internal")]
        private static extern void ShowConsoleMessage(string message);

        private readonly string _emptySaveData = "{}";
#endif

        private string _outOfContextErrorMessage = "Стратегия сохранения Яндекса используется вне контекста WebGL";

        public override JObject Load()
        {
#if UNITY_WEBGL
            SendRequestToLoadSaveFromYandexCloud();
            return new JObject();
#else
            throw new System.Exception(_outOfContextErrorMessage);
#endif
        }

        public override void Save(JObject state)
        {
#if UNITY_WEBGL
            string stateJson = state.ToString();
            SendSaveToYandexCloud(stateJson);
#else
        throw new System.Exception(_outOfContextErrorMessage);
#endif
        }

        public override void Delete()
        {
#if UNITY_WEBGL
            SendSaveToYandexCloud(_emptySaveData);
#else
            throw new System.Exception(_outOfContextErrorMessage);
#endif
        }
    }
}
