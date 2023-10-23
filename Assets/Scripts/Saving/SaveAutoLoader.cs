using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace HCGame.Saving
{
    public class SaveAutoLoader : MonoBehaviour
    {
        private JsonSavingWrapper _savingWrapper;

        [Inject]
        private void Construct(JsonSavingWrapper savingWrapper)
        {
            _savingWrapper = savingWrapper;
        }

        private void Start()
        {
            _savingWrapper.Load();
        }
    }
}
