using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace HCGame.Advertisements
{
    public class FirebaseInitializerService : MonoInstaller
    {
        [SerializeField] private FirebaseInitializer _firebaseInitializerPrefab;

        public override void InstallBindings()
        {
#if UNITY_ANDROID
            FirebaseInitializerInstaller();
#endif
        }

#if UNITY_ANDROID
        private void FirebaseInitializerInstaller()
        {
            FirebaseInitializer firebaseInitializer = Container.InstantiatePrefabForComponent<FirebaseInitializer>(_firebaseInitializerPrefab);
            Container.Bind<FirebaseInitializer>().FromInstance(firebaseInitializer).AsSingle().NonLazy();
        }
#endif
    }
}
