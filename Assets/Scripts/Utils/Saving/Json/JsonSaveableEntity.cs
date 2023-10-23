using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Utils.Saving.Json
{
    [RequireComponent(typeof(UniqueIdentificatorSetter))]
    public class JsonSaveableEntity : MonoBehaviour, IUniqueIdentifiable
    {
        [SerializeField] private string _uniqueIdentifier = "";

        public string UniqueIdentifier => _uniqueIdentifier;

        public string GetUniqueIdentificator()
        {
            return _uniqueIdentifier;
        }

        public string GetUniqueIdentifivatorVariableName()
        {
            return nameof(_uniqueIdentifier);
        }

        public JToken CaptureAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDictionary = state;

            foreach (IJsonSaveable jsonSaveable in GetComponents<IJsonSaveable>())
            {
                JToken token = jsonSaveable.CaptureAsJToken();
                string componentName = jsonSaveable.GetType().ToString();
                stateDictionary[componentName] = token;
            }

            return state;
        }


        public void RestoreFromToken(JToken token)
        {
            JObject state = token.ToObject<JObject>();

            IDictionary<string, JToken> stateDictionary = state;

            foreach (IJsonSaveable jsonSaveable in GetComponents<IJsonSaveable>())
            {
                string componentName = jsonSaveable.GetType().ToString();

                if (!stateDictionary.ContainsKey(componentName))
                    continue;

                jsonSaveable.RestoreFromJToken(stateDictionary[componentName]);
            }
        }
    }
}
