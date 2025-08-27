using deVoid.Utils;
using UnicoCaseStudy.Managers.Pool;
using UnityEngine;

namespace UnicoCaseStudy.Signal
{
    public class DetachParticleSignal : ASignal<DetachParticleSignalProperties>
    {
    }

    public readonly struct DetachParticleSignalProperties
    {
        public readonly PoolKeys PoolKey;
        public readonly GameObject ParticleGameObject;

        public DetachParticleSignalProperties(PoolKeys poolKey, GameObject particleObject)
        {
            PoolKey = poolKey;
            ParticleGameObject = particleObject;
        }
    }
}
