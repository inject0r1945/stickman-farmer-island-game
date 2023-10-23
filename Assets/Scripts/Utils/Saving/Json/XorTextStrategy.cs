using System;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace Utils.Saving.Json
{
    [CreateAssetMenu(menuName = "SavingStrategies/XorTextFile", fileName = "XorTextFileStrategy")]
    public class XorTextStrategy : JsonFileSavingStrategy
    {
        [Header("Use Context Menu to generate a random key.")]
        [SerializeField] string _key = "TheQuickBrownFoxJumpedOverTheLazyDog";

        private readonly string _fileExtension = ".xortext";

        public override void Save(JObject state)
        {
            string path = GetPathFromSaveFile(_fileName);
            Debug.Log($"Saving to {path} ");

            using (var textWriter = File.CreateText(path))
            {
                string json = state.ToString();
                string encoded = EncryptDecrypt(json, _key);
                string base64 = EncodeAsBase64String(encoded);
                textWriter.Write(base64);
            }
        }

        public override JObject Load()
        {
            string path = GetPathFromSaveFile(_fileName);
            if (!File.Exists(path))
            {
                return new JObject();
            }

            using (var textReader = File.OpenText(path))
            {
                string encoded = textReader.ReadToEnd();
                string decoded = DecodeFromBase64String(encoded);
                string json = EncryptDecrypt(decoded, _key);
                return (JObject)JToken.Parse(json);
            }
        }
        public override string GetExtension()
        {
            return _fileExtension;
        }

        private string EncryptDecrypt(string plainText, string encryptionKey)
        {
            StringBuilder inputStringBuild = new StringBuilder(plainText);
            StringBuilder outStringBuild = new StringBuilder(plainText.Length);
            char charText;

            for (int counter = 0; counter < plainText.Length; counter++)
            {
                int stringIndex = counter % encryptionKey.Length;
                charText = inputStringBuild[counter];
                charText = (char)(charText ^ encryptionKey[stringIndex]);
                outStringBuild.Append(charText);
            }
            return outStringBuild.ToString();
        }

        private string EncodeAsBase64String(string source)
        {
            byte[] sourceArray = new byte[source.Length];

            for (int i = 0; i < source.Length; i++)
            {
                sourceArray[i] = (byte)source[i];
            }

            return Convert.ToBase64String(sourceArray);
        }

        private string DecodeFromBase64String(string source)
        {
            byte[] sourceArray = Convert.FromBase64String(source);
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < sourceArray.Length; i++)
            {
                builder.Append((char)sourceArray[i]);
            }

            return builder.ToString();
        }

#if UNITY_EDITOR

        [ContextMenu("Generate Random Key")]
        private void GenerateKey()
        {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty property = serializedObject.FindProperty(nameof(_key));
            property.stringValue = Guid.NewGuid().ToString();
            serializedObject.ApplyModifiedProperties();
        }

#endif
    }
}
