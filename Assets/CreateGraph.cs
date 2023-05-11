using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static Functional;
using static Algorithms;
// ReSharper disable All

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
    public bool showBellmanFord;
    public bool showBellmanFordMoore;
    public bool showColliders;

    private IEnumerator dijkstraCoroutine = null;
    private Color dijkstraColor = Color.yellow;

    private IEnumerator DrawLines()
    {
        List<List<(Vector3 from, Vector3 to)>> linePointLists = new();
        linePointLists.Add(pathGraph.dijkstraLinePoints);

        while (true)
        {
            foreach (var list in linePointLists)
            {
                foreach ((Vector3 from, Vector3 to) in list)
                {
                    Debug.DrawLine(from, to, dijkstraColor);
                }
            }
            yield return null;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Question: How is worldObjects populated? A: in unity
        octree = new Octree(worldObjects, nodeMinSize);
        pathGraph = PathGraph.FromOctree(octree);
        pathGraph.ConnectGraph();
        StartCoroutine(DrawLines());
    }

    private void Update()
    {
        if (!(octree != null && pathGraph.initialized)) return;

        if (showBoxes)
        {
            //DrawWithColor(pathGraph.root.Draw, Color.green);
            pathGraph.root.Draw(Color.green);
        }

        if (showBFS)
        {
            BFS(pathGraph.root, (prev, next, i, len) =>
            {
                Debug.DrawLine(prev.bounds.center, next.bounds.center, Color.red);
            });
        }

        if (showDijkstra && dijkstraCoroutine == null)
        {
            dijkstraCoroutine = Dijkstra(pathGraph);
            StartCoroutine(dijkstraCoroutine);
        }
        else if (!showDijkstra && dijkstraCoroutine != null)
        {
            StopCoroutine(dijkstraCoroutine);
            pathGraph.dijkstraLinePoints.Clear();
            dijkstraCoroutine = null;
        }

        Dictionary<PathGraphNode, double> distUnoptimized = null;
        Dictionary<PathGraphNode, double> distOptimized = null;
        if (showBellmanFord)
        {
            distUnoptimized = BellmanFord(pathGraph);
        }

        if (showBellmanFordMoore)
        {
            distOptimized = BellmanFordMoore(pathGraph);
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
