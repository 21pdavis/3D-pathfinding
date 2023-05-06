using UnityEngine;

using static Functional;

[System.Serializable]
public class CreateGraph : MonoBehaviour
{
    public GameObject[] worldObjects;
    public int nodeMinSize = 1;
    public Octree octree;
    public PathGraph pathGraph;
    public bool showBoxes;
    public bool showSearch;
    public bool showColliders;

    // Start is called before the first frame update
    private void Start()
    {
        // Question: How is worldObjects populated? A: in unity
        octree = new Octree(worldObjects, nodeMinSize);
        pathGraph = PathGraph.FromOctree(octree);
        Debug.Log($"pathGraph nodeCount before connection is {pathGraph.nodeCount}");
        Debug.Log($"pathGraph edgeCount before connection is {pathGraph.edgeCount}");

        pathGraph.ConnectGraph();

        Debug.Log($"pathGraph nodeCount after connection is {pathGraph.nodeCount}");
        Debug.Log($"pathGraph edgeCount after connection is {pathGraph.edgeCount}");
    }

    private void OnDrawGizmos()
    {
        if (!(octree != null && pathGraph.initialized)) return;

        if (showBoxes)
        {
            DrawWithColor(pathGraph.root.Draw, Color.green);
        }

        if (showSearch)
        {
            DrawWithColor(() =>
            {
                Algorithms.BFS(pathGraph.root, (prev, next, i, len) =>
                {
                    Gizmos.DrawLine(prev.bounds.center, next.bounds.center);
                });
            }, Color.red);
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
