using System.Collections.Generic;
using System;
using UnityEngine;
using Frostline.Core;

namespace Frostline.Renderer
{
    public class RendererViewer : MonoBehaviour, IRendererViewer
    {
        private Vector2Int _previousPosition;
        public event Action<Vector2Int> OnMoveEvent;

        private void Start()
        {
            _previousPosition = GetPosition();
            RenderereViewerManager.Instance.AddViewer(this);
        }
        private void OnDestroy()
        {
            RenderereViewerManager.Instance.RemoveViewer(this);
        }
        public Vector2Int GetPosition()
        {
            return new(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        }

        private void Update()
        {
            Vector2Int position = GetPosition();
            int diffX = position.x - _previousPosition.x;
            int diffY = position.y - _previousPosition.y;
            if (diffX * diffX + diffY * diffY > 10)
            {
                OnMoveEvent.Invoke(GetPosition());
                _previousPosition = position;
            }
        }
    }

    public interface IRendererViewer
    {
        event Action<Vector2Int> OnMoveEvent;
        Vector2Int GetPosition();
    }
    public interface IRendererHandler
    {
        void ChangeVisible(Vector2Int[] inRange);
    }

    public class RenderereViewerManager : Singleton<RenderereViewerManager>
    {
        private HashSet<IRendererViewer> _rendererViewers;
        private event Action<Vector2Int[]> _changeVisible;

        public RenderereViewerManager()
        {
            _rendererViewers = new();
        }

        public void AddViewer(IRendererViewer viewer)
        {
            viewer.OnMoveEvent += OnMove;
            _rendererViewers.Add(viewer);
        }
        public void RemoveViewer(IRendererViewer viewer)
        {
            viewer.OnMoveEvent -= OnMove;
            _rendererViewers.Remove(viewer);
        }
        public void AddListener(IRendererHandler handler)
        {
            _changeVisible += handler.ChangeVisible;
        }
        public void RemoveListener(IRendererHandler handler)
        {
            _changeVisible -= handler.ChangeVisible;
        }


        private void OnMove(Vector2Int t)
        {
            List<Vector2Int> inRange = new();
            foreach (IRendererViewer viewer in _rendererViewers)
            {
                inRange.Add(viewer.GetPosition());
            }

            _changeVisible.Invoke(inRange.ToArray());
        }
    }
}
