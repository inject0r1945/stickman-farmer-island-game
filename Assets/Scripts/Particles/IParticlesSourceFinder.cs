using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HCGame.Particles
{
    public interface IParticlesSourceFinder
    {
        public Transform GetParticlesSource();
    }
}
