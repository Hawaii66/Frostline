using System;
using UnityEngine;

namespace Frostline.Renderer
{
    public class RendererViewer : MonoBehaviour, IVisibilityChanged
    {
        private Vector2Int _previousPosition;
        public event Action<Vector2Int> OnVisibilityChanged;

        private void Start()
        {
            _previousPosition = GetPosition();
            VisibilityManager.Instance.AddViewer(this);
        }
        private void OnDestroy()
        {
            VisibilityManager.Instance.RemoveViewer(this);
        }
        private void Update()
        {
            Vector2Int position = GetPosition();
            int diffX = position.x - _previousPosition.x;
            int diffY = position.y - _previousPosition.y;
            if (diffX * diffX + diffY * diffY > 5)
            {
                OnVisibilityChanged.Invoke(GetPosition());
                _previousPosition = position;
            }
        }
        public Vector2Int GetPosition()
        {
            return new(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        }
    }
}
