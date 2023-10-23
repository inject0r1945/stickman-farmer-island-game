using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Utils.Saving
{
    [ExecuteAlways]
    public class UniqueIdentificatorSetter : MonoBehaviour
    {
        private static Dictionary<string, IUniqueIdentifiable> _globalLookup = new Dictionary<string, IUniqueIdentifiable>();

#if UNITY_EDITOR
        private void Update()
        {
            foreach (IUniqueIdentifiable uniqueIdentificatorObject in GetComponents<IUniqueIdentifiable>()) 
                GenerateUniqueIdentifier(uniqueIdentificatorObject);
        }

        private void GenerateUniqueIdentifier(IUniqueIdentifiable uniqueIdentifiable)
        {
            if (Application.IsPlaying(gameObject))
                return;

            if (string.IsNullOrEmpty(gameObject.scene.path))
                return;

            SerializedObject serializedObject = new SerializedObject((UnityEngine.Object)uniqueIdentifiable);
            SerializedProperty property = serializedObject.FindProperty(uniqueIdentifiable.GetUniqueIdentifivatorVariableName());

            if (string.IsNullOrEmpty(property.stringValue) || !IsUniqueIdentifier(uniqueIdentifiable))
            {
                property.stringValue = Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }

            _globalLookup[property.stringValue] = uniqueIdentifiable;
        }

        private bool IsUniqueIdentifier(IUniqueIdentifiable uniqueIdentifiable)
        {
            string uniqueIdentificator = uniqueIdentifiable.GetUniqueIdentificator();

            if (!_globalLookup.ContainsKey(uniqueIdentificator))
                return true;

            IUniqueIdentifiable uniqueIdentifiableCache = _globalLookup[uniqueIdentificator];

            if (uniqueIdentifiableCache == uniqueIdentifiable)
                return true;

            if (uniqueIdentifiableCache as MonoBehaviour == null ||
                uniqueIdentifiableCache.GetUniqueIdentificator() != uniqueIdentificator)
            {
                _globalLookup.Remove(uniqueIdentificator);
                return true;
            }

            return false;
        }
#endif
    }
}

