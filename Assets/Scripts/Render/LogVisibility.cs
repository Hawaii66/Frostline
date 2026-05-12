using Frostline.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Frostline.Renderer
{
    public class LogVisibility : IRequireServices, IVisibilityListener
    {
        private VisibilityManager _visibilityManager;

        public void ChangeVisible(HashSet<Vector2Int> visiblePositions)
        {
            Debug.Log("Change visibility: " + visiblePositions.Count);
        }

        public void Initialize(IServiceRegistry serviceRegistry)
        {
            _visibilityManager = serviceRegistry.GetService<VisibilityManager>();

            _visibilityManager.AddListener(this);
        }
    }
}