using Frostline.Core;
using System;
using UnityEngine;

namespace Frostline.Renderer
{
    public class RendererViewer : MonoBehaviour, IVisibilityViewer, IRequireServices
    {
        private Vector2Int _previousPosition;
        private VisibilityManager _visibilityManager;

        private void Awake()
        {
            _previousPosition = GetPosition();
        }
        private void Start()
        {
            _visibilityManager.OnNotifyListeners();
        }
        private void OnDestroy()
        {
            _visibilityManager.RemoveViewer(this);
        }
        private void Update()
        {
            Vector2Int position = GetPosition();
            int diffX = position.x - _previousPosition.x;
            int diffY = position.y - _previousPosition.y;
            if (diffX * diffX + diffY * diffY > 5)
            {
                _visibilityManager.OnNotifyListeners();
                _previousPosition = position;
            }
        }
        public Vector2Int GetPosition()
        {
            return new(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        }

        public void Initialize(IServiceRegistry serviceRegistry)
        {
            _visibilityManager = serviceRegistry.GetService<VisibilityManager>();
            _visibilityManager.AddViewer(this);
        }
    }
}
