using UnityEngine;
using UnityEngine.UI;

namespace UnicoCaseStudy.Gameplay.Systems
{
    public class DefenceSelectorMover : MonoBehaviour
    {
        [SerializeField] private Image _image;
        private Vector2 _movedItemStartPosition;

        private void Awake()
        {
            _movedItemStartPosition = transform.position;
            gameObject.SetActive(false);
        }

        public void Activate(Sprite sprite, Vector2 position)
        {
            _image.sprite = sprite;
            transform.position = position;
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            transform.position = _movedItemStartPosition;
            gameObject.SetActive(false);
        }

        public void MoveTo(Vector2 position)
        {
            transform.position = position;
        }
    }
}
