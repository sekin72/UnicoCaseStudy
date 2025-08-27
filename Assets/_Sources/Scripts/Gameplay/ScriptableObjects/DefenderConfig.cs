using UnicoCaseStudy.Managers.Pool;
using UnityEngine;

namespace GameClient.GameData
{
    [CreateAssetMenu(fileName = "DefenderConfig", menuName = "UnicoCaseStudy/GameConfig/DefenderConfig", order = 3)]
    public class DefenderConfig : ScriptableObject
    {
        public PoolKeys PoolKey;
        public Sprite Sprite;

        public int Damage;
        public int Range = 1;
        public float Cooldown;
        public Vector3Int Direction = Vector3Int.forward;
    }
}
