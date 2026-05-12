using Frostline.World.Tiles;
using UnityEngine;

namespace Frostline.Renderer
{
    public class FloorTile : MonoBehaviour, IPoolable
    {
        private MeshRenderer _meshRenderer;
        private MaterialPropertyBlock _materialPropertyBlock;
        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _materialPropertyBlock = new();
        }
        public void OnEndUse()
        {
            gameObject.SetActive(false);
        }

        public void OnStartUse()
        {
            gameObject.SetActive(true);
        }

        public void Configure(Tile tile)
        {
            transform.position = new Vector3(tile.X, tile.Height, tile.Y);

            _meshRenderer.GetPropertyBlock(_materialPropertyBlock);
            _materialPropertyBlock.SetColor("_BaseColor", tile.Color);
            _meshRenderer.SetPropertyBlock(_materialPropertyBlock);
        }
    }
}