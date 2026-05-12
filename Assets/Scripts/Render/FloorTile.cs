using UnityEngine;

namespace Frostline.Renderer
{
    public class FloorTile : MonoBehaviour, IPoolable
    {
        public void OnEndUse()
        {
            gameObject.SetActive(false);
        }

        public void OnStartUse()
        {
            gameObject.SetActive(true);
        }

        public void Configure(Vector2Int position)
        {
            transform.position = new Vector3(position.x, 0, position.y);
        }
    }
}