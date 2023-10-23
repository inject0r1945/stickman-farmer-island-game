using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HCGame.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasFadeOuter : MonoBehaviour
    {
        [SerializeField] private float _fadeTime = 0.8f;
        [SerializeField] private float _delay = 1.5f;
        [SerializeField] private bool _isDisableAfterFade = true;

        private CanvasGroup _canvasGroup;
        private float _maxAlpha = 1f;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            _canvasGroup.alpha = _maxAlpha;
            StartCoroutine(FadeOutCoroutine());
        }

        private IEnumerator FadeOutCoroutine()
        {
            float timer = 0;

            while (timer < _delay)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            timer = _fadeTime;

            while (_canvasGroup.alpha > 0)
            {
                _canvasGroup.alpha = Mathf.Max(0, timer / _fadeTime);
                timer -= Time.deltaTime;
                yield return null;
            }

            if (_isDisableAfterFade)
            {
                enabled = false;
            }
        }
    }
}
