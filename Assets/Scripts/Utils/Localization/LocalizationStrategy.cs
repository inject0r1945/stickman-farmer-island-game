using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Localization
{
    public abstract class LocalizationStrategy : ScriptableObject
    {
        public abstract Language GetLanguage();
    }
}
