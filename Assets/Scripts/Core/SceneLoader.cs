using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace HCGame.Core
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private Image _loaderFillImage;
        [SerializeField] private string _sceneName = "Main";
        [SerializeField] private float _loadTime = 2f;
        [SerializeField] private bool _isAutoCompleteLoad = true;

        private AsyncOperation _sceneLoadOperation;

        public UnityEvent LoadedBeforeActivation;

        private void Start()
        {
            StartCoroutine(LoadSceneCoroutine(_sceneName));
        }

        public void CompleteLoad()
        {
            if (_sceneLoadOperation != null)
                _sceneLoadOperation.allowSceneActivation = true;
        }

        private IEnumerator LoadSceneCoroutine(string sceneName)
        {
            _sceneLoadOperation = SceneManager.LoadSceneAsync(sceneName);
            _sceneLoadOperation.allowSceneActivation = false;

            float timer = 0;
            float loadCompleteThreshold = 0.9f;

            while (timer < _loadTime || _sceneLoadOperation.progress < loadCompleteThreshold)
            {
                float progressPercent = timer / _loadTime;
                _loaderFillImage.fillAmount = Mathf.Clamp01(progressPercent);
                timer += Time.deltaTime;

                yield return null;
            }

            LoadedBeforeActivation?.Invoke();

            if (_isAutoCompleteLoad)
                CompleteLoad();
        }
    }
}
