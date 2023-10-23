using HCGame.Attribute;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HCGame.Particles
{
    public class PlayerParticlesSourceFinder : MonoBehaviour, IParticlesSourceFinder
    {
        public Transform GetParticlesSource()
        {
            return Player.CurrentPlayer.transform;
        }
    }
}
