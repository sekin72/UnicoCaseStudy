using UnityEngine;

namespace UnicoCaseStudy.Gameplay.Logic
{
    public class Defender : BoardItem
    {
        public void Initialize(CharacterConfig config, GameplayTile attachedGameplayTile, GameObject idleVFX)
        {
            _sprite = config.Sprite;
            _spriteRenderer.sprite = _sprite;

            IdleVFX = idleVFX;
            IdleVFX.transform.SetParent(_idleVFXParent);
            IdleVFX.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));

            SetAttachedGameplayTile(attachedGameplayTile);
        }
    }
}
