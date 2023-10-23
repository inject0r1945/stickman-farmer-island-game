using UnityEngine;
using System.Runtime.InteropServices;

namespace Utils
{
    public class YandexAPILogger : MonoBehaviour
    {
#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void ShowConsoleMessage(string message);

        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void HandleLog(string logString, string stackTrace, LogType logType)
        {
            string message = "======== UNITY LOG ========\n " + "[" + logType + "] : " + logString;
            ShowConsoleMessage(message);
        }
#endif
    }
}