using UnityEngine;

namespace HCGame.Advertisements
{
    [CreateAssetMenu(fileName = "AppodealConfiguration", menuName = "Advertisements/Appodeal")]
    public class AppodealConfiguration : ScriptableObject
    {
        [SerializeField] private string _applicationKey;
        [SerializeField] private bool _isEnableTestMode = false;
        [SerializeField] private AppodealLogMode _logMode;

        public string ApplicationKey => _applicationKey;

        public bool IsEnableTestMode => _isEnableTestMode;

        public AppodealLogMode LogMode => _logMode;
    }
}