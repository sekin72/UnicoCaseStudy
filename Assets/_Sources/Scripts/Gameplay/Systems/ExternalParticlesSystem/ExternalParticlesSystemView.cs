using System.Collections.Generic;
using UnicoCaseStudy.Managers.Pool;
using UnityEngine;

namespace UnicoCaseStudy.Gameplay.Systems.ExternalParticles
{
    public class ExternalParticlesSystemView : MonoBehaviour
    {
        private readonly Dictionary<PoolKeys, List<GameObject>> _particles = new();

        public void AttachGameObject(PoolKeys key, GameObject particleGameObject)
        {
            if (!_particles.ContainsKey(key))
            {
                _particles.Add(key, new List<GameObject>());
            }

            _particles[key].Add(particleGameObject);
            particleGameObject.transform.SetParent(transform);
        }

        public void DetachGameObject(PoolKeys key, GameObject particleGameObject)
        {
            if (!_particles.ContainsKey(key))
            {
                throw new KeyNotFoundException($"No particles found for key {key}");
            }

            _particles[key].Remove(particleGameObject);
        }
    }
}