#if UNITY_ANDROID
using Firebase.Analytics;
#endif
using System;
using UnityEngine;

namespace HCGame.Advertisements
{
    public class FirebaseInitializer : MonoBehaviour
    {
        private bool _isReady;

        public bool IsReady => _isReady;

#if UNITY_ANDROID
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    _isReady = true;
                }
                else
                {
                    _isReady = false;
                    Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                }
            });
        }
#endif
    }
}
