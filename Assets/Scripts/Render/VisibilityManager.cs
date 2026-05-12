using System;
using System.Collections.Generic;
using UnityEngine;

namespace Frostline.Renderer
{
    public interface IVisibilityChanged
    {
        event Action<Vector2Int> OnVisibilityChanged;
        Vector2Int GetPosition();
    }
    public interface IHandleVisibilityChange
    {
        void ChangeVisible(HashSet<Vector2Int> visiblePositions);
    }

    public class VisibilityManager
    {
        public static VisibilityManager Instance;

        private readonly HashSet<IVisibilityChanged> _rendererViewers;
        private event Action<HashSet<Vector2Int>> _changeVisible;

        private static Vector2Int[] _viewOffsets;

        public VisibilityManager(int viewRadius)
        {
            Instance = this;
            _rendererViewers = new();
            _viewOffsets = CalculateViewOffsets(viewRadius);
        }

        private Vector2Int[] CalculateViewOffsets(int viewRadius)
        {
            List<Vector2Int> offsets = new();
            for (int x = -viewRadius; x < viewRadius; x++)
            {
                for (int y = -viewRadius; y < viewRadius; y++)
                {
                    if (x * x + y * y <= viewRadius * viewRadius)
                    {
                        offsets.Add(new Vector2Int(x, y));
                    }
                }
            }

            return offsets.ToArray();
        }

        public void AddViewer(IVisibilityChanged viewer)
        {
            viewer.OnVisibilityChanged += OnMove;
            _rendererViewers.Add(viewer);
        }
        public void RemoveViewer(IVisibilityChanged viewer)
        {
            viewer.OnVisibilityChanged -= OnMove;
            _rendererViewers.Remove(viewer);
        }
        public void AddListener(IHandleVisibilityChange handler)
        {
            _changeVisible += handler.ChangeVisible;
        }
        public void RemoveListener(IHandleVisibilityChange handler)
        {
            _changeVisible -= handler.ChangeVisible;
        }

        private void OnMove(Vector2Int t)
        {
            HashSet<Vector2Int> inRange = new();
            foreach (IVisibilityChanged viewer in _rendererViewers)
            {
                Vector2Int viewerPosition = viewer.GetPosition();
                for (int i = 0; i < _viewOffsets.Length; i++)
                {
                    inRange.Add(viewerPosition + _viewOffsets[i]);
                }
            }

            _changeVisible.Invoke(inRange);
        }
    }
}
