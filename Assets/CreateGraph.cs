using System.Collections;
using System.Collections.Generic;
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

    private IEnumerator bfsCoroutine = null;
    private IEnumerator dijkstraCoroutine = null;
    private IEnumerator bellmanFordCoroutine = null;
    private IEnumerator bellmanFordMooreCoroutine = null;

    private IEnumerator DrawLines()
    {
        Dictionary<List<(Vector3 from, Vector3 to)>, Color> linePointLists = new()
        {
            { pathGraph.bfsLines, pathGraph.bfsColor },
            { pathGraph.dijkstraLines, pathGraph.dijkstraColor },
            { pathGraph.bellmanFordLines, pathGraph.bellmanFordColor },
            { pathGraph.bellmanFordMooreLines, pathGraph.bellmanFordMooreColor }
        };

        while (true)
        {
            foreach (var kv in linePointLists)
            {
                foreach ((Vector3 from, Vector3 to) in kv.Key)
                {
                    Debug.DrawLine(from, to, kv.Value);
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

        if (showBFS && bfsCoroutine == null)
        {
            bfsCoroutine = CoroutineBFS(pathGraph.root, (prev, next, i, len) =>
            {
                pathGraph.bfsLines.Add((prev.bounds.center, next.bounds.center));
                //Debug.DrawLine(prev.bounds.center, next.bounds.center, Color.red);
            });
            StartCoroutine(bfsCoroutine);
        }
        else if (!showBFS && bfsCoroutine != null)
        {
            StopCoroutine(bfsCoroutine);
            pathGraph.bfsLines.Clear();
            bfsCoroutine = null;
        }

        if (showDijkstra && dijkstraCoroutine == null)
        {
            dijkstraCoroutine = Dijkstra(pathGraph);
            StartCoroutine(dijkstraCoroutine);
        }
        else if (!showDijkstra && dijkstraCoroutine != null)
        {
            StopCoroutine(dijkstraCoroutine);
            pathGraph.dijkstraLines.Clear();
            dijkstraCoroutine = null;
        }

        if (showBellmanFord && bellmanFordCoroutine == null)
        {
            bellmanFordCoroutine = BellmanFord(pathGraph);
            StartCoroutine(bellmanFordCoroutine);
        }
        else if (!showBellmanFord && bellmanFordCoroutine != null)
        {
            StopCoroutine(bellmanFordCoroutine);
            pathGraph.bellmanFordLines.Clear();
            bellmanFordCoroutine = null;
        }

        if (showBellmanFordMoore && bellmanFordMooreCoroutine == null)
        {
            bellmanFordMooreCoroutine = BellmanFordMoore(pathGraph);
            StartCoroutine(bellmanFordMooreCoroutine);
        }
        else if (!showBellmanFordMoore && bellmanFordMooreCoroutine != null)
        {
            StopCoroutine(bellmanFordMooreCoroutine);
            pathGraph.bellmanFordMooreLines.Clear();
            bellmanFordMooreCoroutine = null;
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
