using UnityEngine;
using Zenject;

namespace HCGame.Advertisements
{
    public class AppodealInitializerService : MonoInstaller
    {
        [SerializeField] private AppodealInitializer _appodealInitializer;

        public override void InstallBindings()
        {
#if UNITY_ANDROID
            AppodealInitializer appodealInitializer = Container.InstantiatePrefabForComponent<AppodealInitializer>(_appodealInitializer);
            Container.Bind<AppodealInitializer>().FromInstance(appodealInitializer).AsSingle();
#endif
        }
    }
}
