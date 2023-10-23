using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HCGame.Particles
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticlesMover : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _delayMoveEffect = 1f;
        [SerializeField] private bool _useCameraDistanceCorrection;
        
        private IParticlesSourceFinder _particlesSourceCondition;
        private ParticleSystem _particleSystem;
        private Transform _particlesSource;
        private Coroutine _moveCoroutine;
        private Camera _camera;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            _particlesSourceCondition = GetComponent<IParticlesSourceFinder>();
            _camera = Camera.main;
            InitializeParticlesSource();
        }

        private void InitializeParticlesSource()
        {
            if (_particlesSourceCondition != null)
                _particlesSource = _particlesSourceCondition.GetParticlesSource();
            else
                _particlesSource = transform;
        }

        [NaughtyAttributes.Button]
        private void PlayParticles()
        {
            int amount = 15;
            PlayParticles(amount);
        }

        public void PlayParticles(int amount)
        {
            if (_moveCoroutine != null)
                return;

            _particleSystem.transform.position = _particlesSource.position;

            InitializeEmissionModule();
            InitializeBursts(amount);

            _particleSystem.Play();

            _moveCoroutine = StartCoroutine(nameof(MoveParticlesToTarget), amount);
        }

        private void InitializeEmissionModule()
        {
            ParticleSystem.EmissionModule emissionModule = _particleSystem.emission;
            emissionModule.rateOverTime = 0;
            emissionModule.rateOverDistance = 0;
        }

        private void InitializeBursts(int amount)
        {
            ParticleSystem.Burst[] particlesBursts = new ParticleSystem.Burst[_particleSystem.emission.burstCount];
            _particleSystem.emission.GetBursts(particlesBursts);

            for (int particlesBurstIndex = 0; particlesBurstIndex < particlesBursts.Length; particlesBurstIndex++)
            {
                ParticleSystem.Burst particlesBurst = _particleSystem.emission.GetBurst(particlesBurstIndex);

                if (particlesBurstIndex == 0)
                {
                    particlesBurst.count = amount;
                }
                else
                {
                    particlesBurst.count = 0;
                }

                _particleSystem.emission.SetBurst(particlesBurstIndex, particlesBurst);
            }
        }

        private IEnumerator MoveParticlesToTarget(int amount)
        {
            yield return new WaitForSeconds(_delayMoveEffect);

            ParticleSystem.MainModule mainModule = _particleSystem.main;
            ParticleSystem.MinMaxCurve originalGravityModifier = mainModule.gravityModifier;

            mainModule.gravityModifier = 0;

            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[amount];

            Vector3 targetPosition;

            if (_useCameraDistanceCorrection)
            {
                Vector3 directionTotarget = (_target.transform.position - _camera.transform.position).normalized;
                float distanceToParticles = Vector3.Distance(_camera.transform.position, _particleSystem.transform.position);
                targetPosition = _camera.transform.position + directionTotarget * distanceToParticles;
            }
            else
            {
                targetPosition  = _target.transform.position;
            }

            while (_particleSystem.isPlaying)
            {
                _particleSystem.GetParticles(particles);

                for (int i = 0; i < particles.Length; i++)
                {
                    particles[i].position = Vector3.MoveTowards(particles[i].position,
                        targetPosition, Time.deltaTime * _moveSpeed);
                }

                _particleSystem.SetParticles(particles);

                yield return null;
            }

            mainModule.gravityModifier = originalGravityModifier;
            _moveCoroutine = null;
        }
    }
}
