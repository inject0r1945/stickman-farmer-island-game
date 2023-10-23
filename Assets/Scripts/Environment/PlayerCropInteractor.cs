using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HCGame.Environment
{
    public class PlayerCropInteractor : MonoBehaviour
    {
        [SerializeField] private Material[] _cropMaterials;

        private readonly string _playerPositionParameterName = "_PlayerPosition";

        private void Update()
        {
            foreach (Material cropMaterial in _cropMaterials)
            {
                cropMaterial.SetVector(_playerPositionParameterName, transform.position);
            }
        }
    }
}