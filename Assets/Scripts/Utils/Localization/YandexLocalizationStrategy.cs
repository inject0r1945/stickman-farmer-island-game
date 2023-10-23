using System.Runtime.InteropServices;
using UnityEngine;

namespace Utils.Localization
{
    [CreateAssetMenu(fileName = "YandexLocalizationStrategy", menuName = "Localization/YandexLocalizationStrategy")]
    public class YandexLocalizationStrategy : LocalizationStrategy
    {
#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern string GetLanguageFromYandexSDK();

        [DllImport("__Internal")]
        private static extern void ShowConsoleMessage(string message);

        private const string RussianLanguageCode = "ru";
        private const string EnglishLanguageCode = "en";
        private const string TurkishLanguageCode = "tr";
#endif

        public override Language GetLanguage()
        {
            Language resultLanguage = Language.English;
#if UNITY_WEBGL
            string yandexlanguageName = GetLanguageFromYandexSDK();

            ShowConsoleMessage($"Получен код языка от Яндекса: {yandexlanguageName}");

            switch (yandexlanguageName)
            {
                case RussianLanguageCode:
                    resultLanguage = Language.Russian;
                    break;

                case EnglishLanguageCode:
                    resultLanguage = Language.English;
                    break;

                case TurkishLanguageCode:
                    resultLanguage = Language.Turkish;
                    break;

                default:
                    resultLanguage = Language.English;
                    break;
            }
#endif
            return resultLanguage;
        }
    }
}

