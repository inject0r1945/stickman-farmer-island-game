using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HCGame.Core
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticlesCollisionsHandler : MonoBehaviour
    {
        private ParticleSystem _particleSystem;

        public event UnityAction<Vector3[]> ParticlesCollided;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void OnParticleCollision(GameObject other)
        {
            List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
            int colissionAmount = _particleSystem.GetCollisionEvents(other, collisionEvents);

            Vector3[] colissionPositions = new Vector3[colissionAmount];

            for (int i = 0; i < colissionAmount; i++)
            {
                colissionPositions[i] = collisionEvents[i].intersection;
            }

            ParticlesCollided?.Invoke(colissionPositions);
        }
    }
}
