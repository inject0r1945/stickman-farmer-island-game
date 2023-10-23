using I2.Loc;
using UnityEngine;

namespace Utils.Localization
{
    public class LocalizationTool : MonoBehaviour
    {
        private const string EnglishLanguageCode = "en";
        private const string RussianLanguageCode = "ru";
        private const string TurkishLanguageCode = "tr";

        public static Language GetCurrentLanguage()
        {
            Language currentLanguage;

            switch(LocalizationManager.CurrentLanguageCode)
            {
                case RussianLanguageCode:
                    currentLanguage = Language.Russian;
                    break;

                case EnglishLanguageCode:
                    currentLanguage = Language.English;
                    break;

                case TurkishLanguageCode:
                    currentLanguage = Language.Turkish;
                    break;

                default:
                    currentLanguage = Language.English;
                    break;
            }

            return currentLanguage;
        }
    }
}
