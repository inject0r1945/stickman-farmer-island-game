using UnityEngine;
#if UNITY_ANDROID
using AppodealStack.Monetization.Api;
using AppodealStack.Monetization.Common;
#endif

namespace HCGame.Advertisements
{
    public class AppodealInitializer : MonoBehaviour
    {
        [SerializeField] private AppodealConfiguration _appodealConfiguration;

#if UNITY_ANDROID
        private int _advertisimentTypes = AppodealAdType.RewardedVideo | AppodealAdType.Banner | AppodealAdType.Interstitial;

        private void Start()
        {
            if (Appodeal.IsInitialized(_advertisimentTypes))
                return;

            Appodeal.SetAutoCache(_advertisimentTypes, true);

            Appodeal.SetTesting(_appodealConfiguration.IsEnableTestMode);

            if (_appodealConfiguration.LogMode == AppodealLogMode.Debug)
                Appodeal.SetLogLevel(AppodealLogLevel.Debug);
            else if (_appodealConfiguration.LogMode == AppodealLogMode.Verbose)
                Appodeal.SetLogLevel(AppodealLogLevel.Verbose);

            if (!Appodeal.IsInitialized(_advertisimentTypes))
                Appodeal.Initialize(_appodealConfiguration.ApplicationKey, _advertisimentTypes);
        }
#endif
    }
}
