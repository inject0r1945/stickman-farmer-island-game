using System.Runtime.InteropServices;
using UnityEngine;

namespace Utils.Localization
{
    [CreateAssetMenu(fileName = "StaticLocalizationStrategy", menuName = "Localization/StaticLocalizationStrategy")]
    public class StaticLocalizationStrategy : LocalizationStrategy
    {
        [SerializeField] private Language _languade;

        public override Language GetLanguage()
        {
            return _languade;
        }
    }
}

