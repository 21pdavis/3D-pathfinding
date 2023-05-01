using UnityEngine;

using static Functional;

[System.Serializable]
public class CreateGraph : MonoBehaviour
{
    public GameObject[] worldObjects;
    public int nodeMinSize = 1;
    public Octree octree;
    public PathGraph pathGraph;
    public bool showSearch;
    public bool showColliders;

    // Start is called before the first frame update
    private void Start()
    {
        // Question: How is worldObjects populated? A: in unity
        octree = new Octree(worldObjects, nodeMinSize);
        pathGraph = PathGraph.FromOctree(octree);
    }

    private void OnDrawGizmos()
    {
        if (!(octree != null && octree.initialized)) return;

        //DrawWithColor(octree.root.Draw, Color.green);
        DrawWithColor(pathGraph.root.Draw, Color.green);

        if (showSearch)
        {
            //DrawWithColor(() =>
            //{
            //    Algorithms.BFS(octree.root, (OctreeNode prev, OctreeNode next) =>
            //    {
            //        Gizmos.DrawLine(prev.bounds.center, next.bounds.center);
            //    });
            //}, Color.red);

            DrawWithColor(() =>
            {
                Algorithms.BFS(pathGraph.root, (PathGraphNode prev, PathGraphNode next) =>
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
