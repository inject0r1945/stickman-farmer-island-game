using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace Utils.Controller
{
    class MobileJoystick: MonoBehaviour
    {
        [Header("Elements")]
        [SerializeField] private RectTransform _joystickOutline;
        [SerializeField] private RectTransform _joystickKnob;

        [Header("Settings")]
        [SerializeField] private float _moveFactor;

        private bool _isCanControl;
        private Vector3 _clickedPosition;
        private Vector3 _moveVector;
        private CanvasScaler canvasScaler;

        private Func<bool> IsTouchUp = () => Input.GetMouseButtonUp(0);  

        public Vector3 MoveVector => _moveVector;

        private void Awake()
        {
            canvasScaler = GetComponentInParent<CanvasScaler>();
        }

        private void OnEnable()
        {
            Hide();
        }

        private void OnDisable()
        {
            Hide();
        }

        private void Start()
        {
            Hide();
        }

        private void Update()
        {
            if (_isCanControl)
                Control();
        }

        public void OnClickJoystickZone()
        {
            _clickedPosition = Input.mousePosition;
            _joystickOutline.position = _clickedPosition;

            Show();
        }

        private void Hide()
        {
            _isCanControl = false;
            _joystickOutline.gameObject.SetActive(false);
            _moveVector = Vector3.zero;
        }

        private void Show()
        {
            _isCanControl = true;
            _joystickOutline.gameObject.SetActive(true);
        }

        private void Control()
        {
            Vector3 currentPosition = Input.mousePosition;
            Vector3 direction = currentPosition - _clickedPosition;

            float widthScaleCoefficient = 1;

            if (canvasScaler != null && canvasScaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
                widthScaleCoefficient = Screen.width / canvasScaler.referenceResolution.x;
            }

            float moveMagnitude = direction.magnitude * _moveFactor / Screen.width;

            int halfDevider = 2;
            float maxMoveMagnitude = _joystickOutline.rect.width * widthScaleCoefficient / halfDevider;
            moveMagnitude = Mathf.Min(moveMagnitude, maxMoveMagnitude);

            _moveVector = direction.normalized * moveMagnitude;
            Vector3 targetPosition = _clickedPosition + _moveVector;

            _joystickKnob.position = targetPosition;

            if (IsTouchUp())
            {
                Hide();
            }
        }
    }
}
