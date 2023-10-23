using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace HCGame.Saving
{
    public class SaveController : MonoBehaviour
    {
        private JsonSavingWrapper _jsonSavingWrapper;

        [Inject]
        private void Construct(JsonSavingWrapper jsonSavingWrapper)
        {
            _jsonSavingWrapper = jsonSavingWrapper;
        }

        public void Delete()
        {
            _jsonSavingWrapper.Delete();
        }
    }
}
