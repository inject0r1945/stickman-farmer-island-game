using HCGame.Abilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HCGame.Control
{
    public class PlayerAnimator : MonoBehaviour
    {
        [Header("Elements")]
        [SerializeField] private Animator _animator;

        [Header("Settings")]
        [SerializeField] private float _moveSpeedMultiplier = 40f;

        private readonly string _runStateName = "Run";
        private readonly string _idleStateName = "Idle";
        private readonly string _moveSpeedParameterName = "moveSpeed";
        private readonly int _sowLayerIndex = 1;
        private readonly int _waterLayerIndex = 2;
        private readonly int _harvestLayerIndex = 3;
        private readonly float _maxLayerWeight = 1f;
        private int _runStateHash;
        private int _idleStateHash;
        private int _moveSpeedParameterHash;
        private AnimatorStateInfo _harvestAnimatorStateInfo;
        private Coroutine _stopHarvestAnimationCoroutine;


        private void Awake()
        {
            _runStateHash = Animator.StringToHash(_runStateName);
            _idleStateHash = Animator.StringToHash(_idleStateName);
            _moveSpeedParameterHash = Animator.StringToHash(_moveSpeedParameterName);
        }

        public void UpdateAnimations(Vector3 moveVector)
        {
            if (moveVector.magnitude > 0)
            {
                _animator.SetFloat(_moveSpeedParameterHash, moveVector.magnitude * _moveSpeedMultiplier);
                PlayRunAnimation();

                _animator.transform.forward = moveVector.normalized;
            }
            else
            {
                PlayIdleAnimation();
            }
        }

        public void PlaySowAnimation()
        {
            _animator.SetLayerWeight(_sowLayerIndex, _maxLayerWeight);
        }

        public void StopSowAnimation()
        {
            _animator.SetLayerWeight(_sowLayerIndex, 0);
        }

        public void PlayWaterAnimation()
        {
            _animator.SetLayerWeight(_waterLayerIndex, _maxLayerWeight);
        }

        public void StopWaterAnimation()
        {
            _animator.SetLayerWeight(_waterLayerIndex, 0);
        }

        public void PlayHarvestAnimation()
        {
            _animator.SetLayerWeight(_harvestLayerIndex, _maxLayerWeight);
        }

        public void StopHarvestAnimation()
        {
            if (_stopHarvestAnimationCoroutine == null)
                _stopHarvestAnimationCoroutine = StartCoroutine(nameof(StopHarvestAnimationCoroutine));
        }

        private void PlayRunAnimation()
        {
            _animator.Play(_runStateHash);
        }

        private void PlayIdleAnimation()
        {
            _animator.Play(_idleStateHash);
        }

        private IEnumerator StopHarvestAnimationCoroutine()
        {
            bool isEnd = false;
            float startedAnimationLoopNumber = -Mathf.Infinity;
            float currentAnimationLoopNumber;

            while (!isEnd)
            {
                _harvestAnimatorStateInfo = _animator.GetCurrentAnimatorStateInfo(_harvestLayerIndex);
                currentAnimationLoopNumber = (int)_harvestAnimatorStateInfo.normalizedTime;

                if (startedAnimationLoopNumber < 0)
                {
                    startedAnimationLoopNumber = currentAnimationLoopNumber;
                    yield return null;
                }

                if (currentAnimationLoopNumber > startedAnimationLoopNumber)
                    isEnd = true;

                yield return null;
            }

            _animator.SetLayerWeight(_harvestLayerIndex, 0);
            _stopHarvestAnimationCoroutine = null;
        }
    }
}