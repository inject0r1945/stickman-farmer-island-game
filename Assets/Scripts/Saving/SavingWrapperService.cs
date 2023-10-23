using HCGame.Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace HCGame.Saving
{
    public class JsonSavingWrapperService : MonoInstaller
    {
        [SerializeField] private JsonSavingWrapper _jsonSavingWrapperPrefab;

        public override void InstallBindings()
        {
            JsonSavingWrapperInstaller();
        }

        private void JsonSavingWrapperInstaller()
        {
            JsonSavingWrapper jsonSavingWrapper = Container.InstantiatePrefabForComponent<JsonSavingWrapper>(_jsonSavingWrapperPrefab);
            Container.Bind<JsonSavingWrapper>().FromInstance(jsonSavingWrapper).AsSingle();
        }
    }
}

