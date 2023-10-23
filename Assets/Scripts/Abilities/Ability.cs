using HCGame.Control;
using HCGame.Core;
using HCGame.Environment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HCGame.Abilities
{
    public abstract class Ability : MonoBehaviour
    {
        [SerializeField] private ParticlesCollisionsHandler _particlesColissionsHandler;

        private PlayerAnimator _playerAnimator;
        private PlayerTools _playerTools;
        private CropField _currentCropField;

        protected PlayerAnimator PlayerAnimator => _playerAnimator;

        protected PlayerTools PlayerTools => _playerTools;

        protected CropField CurrentCropField => _currentCropField;


        private void Awake()
        {
            _playerAnimator = GetComponent<PlayerAnimator>();
            _playerTools = GetComponent<PlayerTools>();
        }

        private void Start()
        {
            if (_particlesColissionsHandler != null)
                _particlesColissionsHandler.ParticlesCollided += OnAbilityParticlesCollided;

            _playerTools.ToolChanged += OnToolChanged;
            SubscribeEvents();
        }

        private void OnDestroy()
        {
            if (_particlesColissionsHandler != null)
                _particlesColissionsHandler.ParticlesCollided -= OnAbilityParticlesCollided;

            _playerTools.ToolChanged -= OnToolChanged;
            UnsubscribeEvents();
        }

        private void OnTriggerStay(Collider other)
        {
            OnTriggerEnter(other);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (CanUseAbilitByPlayer() && other.transform.TryGetComponent(out CropField cropField))
            {
                _currentCropField = cropField;

                if (!_currentCropField.IsBlocked && !IsFullyAbilityOnCropField() && CanUseAbilityOnCropField())
                {
                    StartAbilityAnimation();
                    DoAfterStartAbilityAnimation();
                }
                else
                {
                    StopAbilityAnimation();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform.TryGetComponent(out CropField _))
            {
                StopAbilityAnimation();
                _currentCropField = null;
            }
        }

        protected abstract void SubscribeEvents();

        protected abstract void UnsubscribeEvents();

        protected abstract bool CanUseAbilitByPlayer();

        protected abstract bool IsFullyAbilityOnCropField();

        protected abstract bool CanUseAbilityOnCropField();

        protected abstract void StartAbilityAnimation();

        protected abstract void StopAbilityAnimation();

        protected virtual void AbilityParticlesColliderHandler(Vector3[] particlesPositions) 
        {
        }

        protected virtual void DoAfterStartAbilityAnimation()
        {
        }

        protected void OnFullyAbilityAction(CropField cropField)
        {
            if (cropField == _currentCropField)
                StopAbilityAnimation();
        }

        private void OnAbilityParticlesCollided(Vector3[] particlesPositions)
        {
            if (_currentCropField == null)
                return;

            AbilityParticlesColliderHandler(particlesPositions);
        }

        private void OnToolChanged(ToolType currentToolType)
        {
            if (CanUseAbilitByPlayer())
                return;

            StopAbilityAnimation();
        }
    }
}