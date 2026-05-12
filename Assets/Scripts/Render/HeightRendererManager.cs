using Frostline.Core;
using Frostline.World.Tiles;
using System.Collections.Generic;
using UnityEngine;

namespace Frostline.Renderer
{
    public class HeightRendererManager : MonoBehaviour, IVisibilityListener, IRequireServices
    {
        [SerializeField] private FloorTile _floorTilePrefab;
        [SerializeField] private Transform _floorTilePoolHolder;
        private ObjectPool<FloorTile> _floorTilePool;
        private Dictionary<Vector2Int, FloorTile> _visibleFloorTiles;
        private VisibilityManager _visibilityManager;
        private TileManager _tileManager;

        private void Awake()
        {
            _visibleFloorTiles = new();
            _floorTilePool = new(50, InstantiateFloorPrefab);
        }
        private void OnDestroy()
        {
            _visibilityManager.RemoveListener(this);
        }
        private FloorTile InstantiateFloorPrefab()
        {
            GameObject gameObject = Instantiate(_floorTilePrefab.gameObject, _floorTilePoolHolder);
            return gameObject.GetComponent<FloorTile>();
        }

        public void ChangeVisible(HashSet<Vector2Int> visiblePositions)
        {
            List<Vector2Int> outsideVisibleRange = new();

            foreach (KeyValuePair<Vector2Int, FloorTile> keyValuePair in _visibleFloorTiles)
            {
                if (!visiblePositions.Contains(keyValuePair.Key))
                {
                    _floorTilePool.ReturnToPool(keyValuePair.Value);
                    outsideVisibleRange.Add(keyValuePair.Key);
                }
            }

            for (int i = 0; i < outsideVisibleRange.Count; ++i)
            {
                _visibleFloorTiles.Remove(outsideVisibleRange[i]);
            }

            foreach (Vector2Int pos in visiblePositions)
            {
                if (_visibleFloorTiles.ContainsKey(pos))
                {
                    continue;
                }

                FloorTile floorTile = _floorTilePool.GetFromPool();
                floorTile.Configure(_tileManager.GetTile(pos));
                _visibleFloorTiles.Add(pos, floorTile);
            }
        }

        public void Initialize(IServiceRegistry serviceRegistry)
        {
            _visibilityManager = serviceRegistry.GetService<VisibilityManager>();
            _visibilityManager.AddListener(this);
            _tileManager = serviceRegistry.GetService<TileManager>();
        }
    }
}