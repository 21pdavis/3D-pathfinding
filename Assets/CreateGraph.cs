using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Functional;
using static Algorithms;

[Serializable]
public class CreateGraph : MonoBehaviour
{
    public GameObject[] worldObjects;
    public int NodeMinSize { set; get; }  = 1;
    public Octree Octree { private set; get; }
    public PathGraph PathGraph { private set; get; }

    public bool showBoxes;
    public bool showBfs = false;
    public bool showDijkstra = false;
    public bool showBellmanFord = false;
    public bool showBellmanFordMoore = false;
    public bool showColliders = false;

    private IEnumerator bfsCoroutine;
    private IEnumerator dijkstraCoroutine;
    private IEnumerator bellmanFordCoroutine;
    private IEnumerator bellmanFordMooreCoroutine;

    private IEnumerator DrawLines()
    {
        Dictionary<List<(Vector3 from, Vector3 to)>, Color> linePointLists = new()
        {
            { PathGraph.BfsLines, PathGraph.BfsColor },
            { PathGraph.DijkstraLines, PathGraph.DijkstraColor },
            { PathGraph.BellmanFordLines, PathGraph.BellmanFordColor },
            { PathGraph.BellmanFordMooreLines, PathGraph.BellmanFordMooreColor }
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
            // TODO: Could refactor this to be more proper use of return, but it works so I don't care for now
            yield return null;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Question: How is worldObjects populated? A: in unity
        Octree = new Octree(worldObjects, NodeMinSize);
        PathGraph = PathGraph.FromOctree(Octree);
        PathGraph.ConnectGraph();
        StartCoroutine(DrawLines());
    }

    private void Update()
    {
        if (!(Octree != null && PathGraph.Initialized)) return;

        if (showBoxes)
        {
            //DrawWithColor(pathGraph.root.Draw, Color.green);
            PathGraph.Root.Draw(Color.green);
        }

        if (showBfs && bfsCoroutine == null)
        {
            bfsCoroutine = CoroutineBfs(PathGraph.Root, (prev, next, _, _) =>
            {
                PathGraph.BfsLines.Add((prev.Bounds.center, next.Bounds.center));
                //Debug.DrawLine(prev.bounds.center, next.bounds.center, Color.red);
            });
            StartCoroutine(bfsCoroutine);
        }
        else if (!showBfs && bfsCoroutine != null)
        {
            StopCoroutine(bfsCoroutine);
            PathGraph.BfsLines.Clear();
            bfsCoroutine = null;
        }

        if (showDijkstra && dijkstraCoroutine == null)
        {
            dijkstraCoroutine = Dijkstra(PathGraph);
            StartCoroutine(dijkstraCoroutine);
        }
        else if (!showDijkstra && dijkstraCoroutine != null)
        {
            StopCoroutine(dijkstraCoroutine);
            PathGraph.DijkstraLines.Clear();
            dijkstraCoroutine = null;
        }

        if (showBellmanFord && bellmanFordCoroutine == null)
        {
            bellmanFordCoroutine = BellmanFord(PathGraph);
            StartCoroutine(bellmanFordCoroutine);
        }
        else if (!showBellmanFord && bellmanFordCoroutine != null)
        {
            StopCoroutine(bellmanFordCoroutine);
            PathGraph.BellmanFordLines.Clear();
            bellmanFordCoroutine = null;
        }

        if (showBellmanFordMoore && bellmanFordMooreCoroutine == null)
        {
            bellmanFordMooreCoroutine = BellmanFordMoore(PathGraph);
            StartCoroutine(bellmanFordMooreCoroutine);
        }
        else if (!showBellmanFordMoore && bellmanFordMooreCoroutine != null)
        {
            StopCoroutine(bellmanFordMooreCoroutine);
            PathGraph.BellmanFordMooreLines.Clear();
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
