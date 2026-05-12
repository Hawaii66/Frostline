using System;
using UnityEngine;

namespace Frostline.Renderer
{
    public class HeightRenderer : MonoBehaviour, IRendererHandler
    {
        private ObjectPool<IPoolable> pool;

        private void Start()
        {
            pool = new(5, () => null);
            RenderereViewerManager.Instance.AddListener(this);
        }
        private void OnDestroy()
        {
            RenderereViewerManager.Instance.RemoveListener(this);
        }

        public void ChangeVisible(Vector2Int[] inRange)
        {
            Debug.Log("Change visible" + inRange.Length);
        }
    }
}