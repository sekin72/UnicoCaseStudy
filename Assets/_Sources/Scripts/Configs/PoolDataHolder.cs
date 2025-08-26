using System.Collections.Generic;
using UnityEngine;
using VInspector;

namespace UnicoCaseStudy.Managers.Pool
{
    [CreateAssetMenu(fileName = "PoolDataHolder", menuName = "UnicoCaseStudy/Configs/PoolDataHolder", order = 1)]
    public class PoolDataHolder : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<PoolKeys, PoolDefinition> _poolDataDict = new();

        public PoolDefinition GetPoolDefinition(PoolKeys key)
        {
            return _poolDataDict.GetValueOrDefault(key);

        }
    }
}