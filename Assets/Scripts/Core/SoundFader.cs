using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HCGame.Core
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundFader : MonoBehaviour
    {
        [SerializeField] private bool _playOnStart;
        [SerializeField] private float _fadeInTime = 2f;

        private AudioSource _audio;
        private float _maxVolume;

        private void Awake()
        {
            _audio = GetComponent<AudioSource>();
            _maxVolume = _audio.volume;
        }

        private void Start()
        {
            if (_playOnStart)
                StartCoroutine(nameof(FadeInCoroutine));
        }

        private IEnumerator FadeInCoroutine()
        {
            _audio.volume = 0;
            _audio.Play();

            float timer = 0f;
            bool isEnd = false;

            while (!isEnd)
            {
                timer += Time.deltaTime;
                _audio.volume = Mathf.Min(timer / _fadeInTime, _maxVolume);

                if (timer > _fadeInTime)
                    isEnd = true;

                yield return null;
            }
        }
    }
}
