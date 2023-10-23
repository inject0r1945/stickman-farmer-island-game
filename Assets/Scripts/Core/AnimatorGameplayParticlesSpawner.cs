using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HCGame.Core
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorGameplayParticlesSpawner : MonoBehaviour
    {
        [Header("Elements")]
        [SerializeField] private ParticleSystem _seedsParticles;
        [SerializeField] private ParticleSystem _waterParticles;

        private Animator _animator;
        private readonly int _waterLayerIndex = 2;
        private Coroutine _waterPlayerCoroutine;
        private bool _isPlayWaterParticles;

        public UnityEvent SeedsDropped;

        public UnityEvent StartedHarvesting;

        public UnityEvent StoppedHarvesting;

        public UnityEvent StartedWatering;

        public UnityEvent StoppedWatering;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void SpawnSeedsParticles()
        {
            _seedsParticles.Play();
            SeedsDropped?.Invoke();
        }

        private void SpawnWaterParticles()
        {
            if (_isPlayWaterParticles)
                return;

            _waterPlayerCoroutine = StartCoroutine(nameof(PlayWaterParticlesCoroutine));
        }

        private IEnumerator PlayWaterParticlesCoroutine()
        {
            _waterParticles.Play();
            StartedWatering?.Invoke();
            _isPlayWaterParticles = true;

            while (_animator.GetLayerWeight(_waterLayerIndex) > 0)
                yield return null;

            _waterParticles.Stop();
            _isPlayWaterParticles = false;
            StoppedWatering?.Invoke();
        }

        private void StartHarvesting()
        {
            StartedHarvesting?.Invoke();
        }

        private void StopHarvesting()
        {
            StoppedHarvesting?.Invoke();
        }
    }
}