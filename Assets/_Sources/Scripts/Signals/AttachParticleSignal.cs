using deVoid.Utils;
using UnicoCaseStudy.Managers.Pool;
using UnityEngine;

namespace UnicoCaseStudy.Signal
{
    public class AttachParticleSignal : ASignal<AttachParticleSignalProperties>
    {
    }

    public readonly struct AttachParticleSignalProperties
    {
        public readonly PoolKeys PoolKey;
        public readonly GameObject ParticleGameObject;

        public AttachParticleSignalProperties(PoolKeys poolKey, GameObject particleObject)
        {
            PoolKey = poolKey;
            ParticleGameObject = particleObject;
        }
    }
}
