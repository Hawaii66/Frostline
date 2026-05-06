using NaughtyAttributes;
using UnityEngine;

public class Runner : MonoBehaviour
{
    GraphGeneration graph;

    [Button("Run")]
    void Run()
    {
        Debug.Log("Starting to run world generation");

        graph = new GraphGeneration(5);
        graph.DEBUG_Log();
    }

    private void OnDrawGizmos()
    {
        if (graph != null)
        {
            Gizmos.color = Color.yellow;
            Vector2[] positions = graph.DEBUG_GetNodePositions();
            for (int i = 0; i < positions.Length; i++)
            {
                Gizmos.DrawSphere(new Vector3(positions[i].x, 0, positions[i].y), 0.2f);
            }

            Gizmos.color = Color.blue;
            Vector2[][] edgePositions = graph.DEBUG_GetEdgePositions();
            for(int i = 0; i < edgePositions.Length; i++)
            {
                Vector3 start = new Vector3(edgePositions[i][0].x, 0, edgePositions[i][0].y);
                Vector3 end = new Vector3(edgePositions[i][1].x, 0, edgePositions[i][1].y);
                Gizmos.DrawLine(start, end);
            }
        }
    }
}
