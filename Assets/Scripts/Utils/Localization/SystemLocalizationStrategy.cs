using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Localization
{
    [CreateAssetMenu(fileName = "SystemLocalizationStrategy", menuName = "Localization/SystemLocalizationStrategy")]
    public class SystemLocalizationStrategy : LocalizationStrategy
    {
        public override Language GetLanguage()
        {
            Language resultLanguage;

            switch (Application.systemLanguage)
            {
                case SystemLanguage.Russian:
                    resultLanguage = Language.Russian;
                    break;

                case SystemLanguage.English:
                    resultLanguage = Language.English;
                    break;

                case SystemLanguage.Turkish:
                    resultLanguage = Language.Turkish;
                    break;

                default:
                    resultLanguage = Language.English;
                    break;
            }

            return resultLanguage;
        }
    }
}
