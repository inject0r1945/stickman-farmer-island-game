using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using I2;
using I2.Loc;
using System;

namespace Utils.Localization
{
    public class LocalizationInitializer : MonoBehaviour
    {
        [SerializeField] private LocalizationStrategy _localizationStrategy;

        public UnityEvent <Language>LanguageDetected;

        private void Start()
        {
            SetGameLanguage();
        }

        private void SetGameLanguage()
        {
            Language language = DetectLanguage();
            string languageName = Enum.GetName(typeof(Language), language);

            if (LocalizationManager.HasLanguage(languageName))
            {
                LocalizationManager.CurrentLanguage = languageName;
            }

            Debug.Log($"Установлен язык системы: {languageName}");
        }

        private Language DetectLanguage()
        {
            Language language = _localizationStrategy.GetLanguage();
            LanguageDetected.Invoke(language);

            return language;
        }
    }
}
