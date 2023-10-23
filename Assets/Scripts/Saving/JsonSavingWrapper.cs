using HCGame.Utils.Static;
using Newtonsoft.Json.Linq;
using System.Collections;
using UnityEngine;
using Utils.Saving.Json;

namespace HCGame.Saving
{
    public class JsonSavingWrapper : MonoBehaviour
    {
        [SerializeField] private float _saveTimePeriod = 1f;
        [SerializeField] private bool _isLoadOnStart = true;

        private JsonSavingSystem _savingSystem;
        private bool _isNeedToSave;
        private bool _isEnableSaving = true;

        private void Awake()
        {
            _savingSystem = StaticUtils.GetUniqueComponentOnScene<JsonSavingSystem>();
        }

        private void Start()
        {
            if (_isLoadOnStart)
            {
                _savingSystem.Load();
            }

            EnableSaving();
            StartCoroutine(PeriodicSaveCoroutine());
        }

        public void SendSignalToSave()
        {
            _isNeedToSave = true;
        }

        public void Load(JObject state)
        {
            _savingSystem.Load(state);
        }

        [NaughtyAttributes.Button]
        public void Load()
        {
            _savingSystem.Load();
        }

        private IEnumerator PeriodicSaveCoroutine()
        {
            bool isEnd = false;
            var waitForSeconds = new WaitForSeconds(_saveTimePeriod);

            while (!isEnd)
            {

                if (_isNeedToSave && _isEnableSaving)
                {
                    Save();
                    _isNeedToSave = false;
                }

                yield return waitForSeconds;
            }
        }

        [NaughtyAttributes.Button]
        private void Save()
        {
            _savingSystem.Save();
        }

        [NaughtyAttributes.Button]
        public void Delete()
        {
            _savingSystem.Delete();
        }

        public void DisableSaving()
        {
            _isEnableSaving = false;
        }

        public void EnableSaving()
        {
            _isEnableSaving = true;
        }
    }
}
