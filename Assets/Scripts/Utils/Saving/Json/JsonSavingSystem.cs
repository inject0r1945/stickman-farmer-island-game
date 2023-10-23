using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.IO;

namespace Utils.Saving.Json
{
    public class JsonSavingSystem : MonoBehaviour
    {
        [SerializeField] SavingStrategy _savingStrategy;

        private readonly string _lastSceneIndexName = "lastSceneIndex";

        public IEnumerator LoadLastScene()
        {
            JObject state = LoadFromSystemContainer();
            IDictionary<string, JToken> stateDictionary = state;
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;

            if (stateDictionary.ContainsKey(_lastSceneIndexName))
            {
                sceneIndex = (int)stateDictionary[_lastSceneIndexName];
            }

            yield return SceneManager.LoadSceneAsync(sceneIndex);

            RestoreFromToken(state);
        }

        public void Save()
        {
            JObject currentGameState = GetCurrentGameState();
            SaveToSystemContainer(currentGameState);
        }

        public void Load()
        {
            Load(LoadFromSystemContainer());
        }

        public void Load(JObject state)
        {
            RestoreFromToken(state);
        }

        public void Delete()
        {
            _savingStrategy.Delete();
        }

        private JObject GetCurrentGameState()
        {
            JObject state = new JObject();

            IDictionary<string, JToken> stateDictionary = state;

            foreach (JsonSaveableEntity saveable in FindObjectsOfType<JsonSaveableEntity>())
            {
                stateDictionary[saveable.UniqueIdentifier] = saveable.CaptureAsJToken();
            }

            stateDictionary[_lastSceneIndexName] = SceneManager.GetActiveScene().buildIndex;

            return state;
        }

        private void SaveToSystemContainer(JObject state)
        {
            _savingStrategy.Save(state);
        }

        private JObject LoadFromSystemContainer()
        {
            return _savingStrategy.Load();
        }

        private void RestoreFromToken(JObject state)
        {
            IDictionary<string, JToken> stateDictionary = state;

            foreach (JsonSaveableEntity jsonSaveable in FindObjectsOfType<JsonSaveableEntity>())
            {
                if (!state.ContainsKey(jsonSaveable.UniqueIdentifier))
                    continue;

                jsonSaveable.RestoreFromToken(stateDictionary[jsonSaveable.UniqueIdentifier]);
            }
        }
    }
}
