using System.Collections;
using UnityEngine;
using TMPro;

namespace Utils
{
    public class ScreenLogger : MonoBehaviour
    {
        [SerializeField] private TMP_Text _logText;
        [SerializeField] private int _messagesCount = 15;

        private Queue _logQueue = new Queue();
        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void Start()
        {
            Debug.Log("Started up logging.");
        }

        private void HandleLog(string logString, string stackTrace, LogType logType)
        {
            _logQueue.Enqueue("[" + logType + "] : " + logString);

            if (logType == LogType.Exception)
                _logQueue.Enqueue(stackTrace);

            while (_logQueue.Count > _messagesCount)
                _logQueue.Dequeue();
        }

        private void OnGUI()
        {
            _logText.text = "\n" + string.Join("\n", _logQueue.ToArray());
        }
    }
}
