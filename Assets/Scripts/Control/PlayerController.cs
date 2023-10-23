using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.Controller;
using Utils.Saving.Json;

namespace HCGame.Control
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerAnimator))]
    public class PlayerController : MonoBehaviour, IJsonSaveable
    {
        [Header("Elements")]
        [SerializeField] private MobileJoystick _joystick;

        [Header("Settings")]
        [SerializeField] private float _moveSpeed = 1f;

        private CharacterController _characterController;
        private PlayerAnimator _playerAnimator;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _playerAnimator = GetComponent<PlayerAnimator>();
            
        }

        private void Update()
        {
            Move();
        }

        public JToken CaptureAsJToken()
        {
            return transform.position.ToToken();
        }

        public void RestoreFromJToken(JToken state)
        {
            transform.position = state.ToVector3();
        }

        private void Move()
        {
            Vector3 moveVector = new Vector3(_joystick.MoveVector.x, 0, _joystick.MoveVector.y);

            moveVector = moveVector * _moveSpeed / Screen.width;
            _playerAnimator.UpdateAnimations(moveVector);

            moveVector.y = Physics.gravity.y;

            if (moveVector.magnitude != 0)
                _characterController.Move(moveVector * Time.deltaTime);
        }
    }
}