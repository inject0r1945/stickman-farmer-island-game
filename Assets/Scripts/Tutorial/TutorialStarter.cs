using HCGame.Saving;
using HSVStudio.Tutorial;
using I2.Loc;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.Localization;
using Utils.Saving.Json;
using Zenject;

namespace HCGame.Tutorials
{
    public class TutorialStarter : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] private Tutorial _russianTutorial;
        [SerializeField] private Tutorial _englishTutorial;
        [SerializeField] private Tutorial _turkishTutorial;
        [SerializeField] private bool _isRunOnStart = true;

        private bool _isCompleted;
        private Tutorial _currentTutorial;
        private bool _isStarted;
        private JsonSavingWrapper _savingWrapper;
        private Language _language;

        [Inject]
        private void Construct(JsonSavingWrapper savingSystemWrapper)
        {
            _savingWrapper = savingSystemWrapper;
        }

        private void Awake()
        {
            _language = LocalizationTool.GetCurrentLanguage();
        }

        private void Start()
        {
            if (_isRunOnStart)
            {
                StartTutorial();
            }
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(_isCompleted);
        }

        public void RestoreFromJToken(JToken state)
        {
            _isCompleted = state.ToObject<bool>();

            if (!_isCompleted)
            {
                StartTutorial();
            }
        }

        public void StartTutorial()
        {
            if (_isCompleted || _isStarted)
                return;

            _savingWrapper.DisableSaving();

            switch (_language)
            {
                case Language.Russian:
                    _currentTutorial = _russianTutorial;
                    break;

                case Language.English:
                    _currentTutorial = _englishTutorial;
                    break;

                case Language.Turkish:
                    _currentTutorial = _turkishTutorial;
                    break;

                default:
                    _currentTutorial = _englishTutorial;
                    break;
            }

            _currentTutorial.gameObject.SetActive(true);

            _isStarted = true;
        }

        public void DisableRunOnStart()
        {
            _isRunOnStart = false;
        }

        public void Complete()
        {
            _isCompleted = true;
            _currentTutorial.gameObject.SetActive(false);
            _savingWrapper.EnableSaving();
            _savingWrapper.SendSignalToSave();
        }

        public void SetLanguage(Language language)
        {
            _language = language;
        }
    }
}
