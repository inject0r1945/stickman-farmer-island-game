using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace HCGame.Environment
{
    public class Crop : MonoBehaviour
    {
        [Header("Elements")]
        [SerializeField] private Transform _cropRenderer;
        [SerializeField] private ParticleSystem _destroyParticles;

        [Header("Parameters")]
        [SerializeField, Range(0, 100)] private float _scaleFactor;
        [SerializeField, Range(0, 10)] private float _timeToScale = 1f;
        
        public void ScaleUp()
        {
            _cropRenderer.DOScale(Vector3.one * _scaleFactor, _timeToScale).SetEase(Ease.OutBack);
        }

        public void Destroy()
        {
            _cropRenderer.DOScale(Vector3.zero, _timeToScale)
                .SetEase(Ease.OutBack)
                .OnComplete(() => Destroy(gameObject));

            _destroyParticles.transform.parent = null;
            _destroyParticles.Play();
        }
    }
}
