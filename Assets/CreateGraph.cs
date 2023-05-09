using UnityEngine;

using static Functional;
using static Algorithms;

[System.Serializable]
public class CreateGraph : MonoBehaviour
{
    public GameObject[] worldObjects;
    public int nodeMinSize = 1;
    public Octree octree;
    public PathGraph pathGraph;
    public bool showBoxes;
    public bool showBFS;
    public bool showDijkstra;
    public bool showColliders;

    // Start is called before the first frame update
    private void Start()
    {
        // Question: How is worldObjects populated? A: in unity
        octree = new Octree(worldObjects, nodeMinSize);
        pathGraph = PathGraph.FromOctree(octree);
        pathGraph.ConnectGraph();
    }

    private void OnDrawGizmos()
    {
        if (!(octree != null && pathGraph.initialized)) return;

        if (showBoxes)
        {
            DrawWithColor(pathGraph.root.Draw, Color.green);
        }

        if (showBFS)
        {
            DrawWithColor(() =>
            {
                BFS(pathGraph.root, (prev, next, i, len) =>
                {
                    Gizmos.DrawLine(prev.bounds.center, next.bounds.center);
                });
            }, Color.red);
        }

        if (showDijkstra)
        {
            Dijkstra(pathGraph);
        }

        if (showColliders)
        {
            Gizmos.color = Color.red;
            foreach (GameObject obj in worldObjects)
            {
                Bounds objBounds = obj.GetComponent<Collider>().bounds;
                DrawWireCubeWithColor(objBounds, Color.red);
            }
        }
    }
}
