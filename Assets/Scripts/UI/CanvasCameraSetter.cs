using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HCGame.UI
{
    [RequireComponent(typeof(Canvas))]
    public class CanvasCameraSetter : MonoBehaviour
    {
        private Canvas _canvas;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
        }

        private void Start()
        {
            _canvas.worldCamera = Camera.main;
        }
    }
}
