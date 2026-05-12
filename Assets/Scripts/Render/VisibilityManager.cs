using System;
using System.Collections.Generic;
using UnityEngine;

namespace Frostline.Renderer
{
    public interface IVisibilityViewer
    {
        Vector2Int GetPosition();
    }
    public interface IVisibilityListener
    {
        void ChangeVisible(HashSet<Vector2Int> visiblePositions);
    }

    public class VisibilityManager
    {
        private readonly HashSet<IVisibilityViewer> _rendererViewers;
        private event Action<HashSet<Vector2Int>> _changeVisible;

        private static Vector2Int[] _viewOffsets;

        public VisibilityManager(int viewRadius)
        {
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

        public void AddViewer(IVisibilityViewer viewer)
        {
            _rendererViewers.Add(viewer);
        }
        public void RemoveViewer(IVisibilityViewer viewer)
        {
            _rendererViewers.Remove(viewer);
        }
        public void AddListener(IVisibilityListener handler)
        {
            _changeVisible += handler.ChangeVisible;
        }
        public void RemoveListener(IVisibilityListener handler)
        {
            _changeVisible -= handler.ChangeVisible;
        }

        public void OnNotifyListeners()
        {
            HashSet<Vector2Int> inRange = new();
            foreach (IVisibilityViewer viewer in _rendererViewers)
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
