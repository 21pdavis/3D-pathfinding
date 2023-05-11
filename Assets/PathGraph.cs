using System.Collections.Generic;
using UnityEngine;

using static Algorithms;

public class PathGraph
{
    public PathGraphNode Root { private set; get; }
    public HashSet<PathGraphNode> NodeSet { private set; get; } = new();
    public HashSet<Edge> EdgeSet { private set; get; } = new();
    public bool Initialized { private set; get; }
    public int NodeCount { private set; get; } = 1; // in the current version of the graph, this number should come out to 1337
    public int EdgeCount { private set; get; }

    public List<(Vector3 from, Vector3 to)> DijkstraLines { set; get; } = new();
    public List<(Vector3 from, Vector3 to)> BfsLines { set; get; } = new();
    public List<(Vector3 from, Vector3 to)> BellmanFordLines { set; get; } = new();
    public List<(Vector3 from, Vector3 to)> BellmanFordMooreLines { set; get; } = new();

    public Color BfsColor { set; get; } = Color.red;
    public Color DijkstraColor { set; get; } = Color.yellow;
    public Color BellmanFordColor { set; get; } = new(1, 0, 1);
    public Color BellmanFordMooreColor { set; get; } = Color.cyan;

    // initialize PathGraph using a level-order traversal of the given octree
    public static PathGraph FromOctree(Octree octree)
    {
        PathGraph ret = new();

        Queue<PathGraphNode> queue = new();
        PathGraphNode prevPathGraphNode = null;
        OctreeNode octreePrev = null;
        PathGraphNode pathRoot = null;

        Bfs(octree.Root, (prev, next) =>
        {
            if (octreePrev != prev)
            {
                if (prev == octree.Root)
                {
                    pathRoot = PathGraphNode.FromOctreeNode(prev);
                    ret.NodeSet.Add(pathRoot);
                    queue.Enqueue(pathRoot);
                }
                prevPathGraphNode = queue.Dequeue();
                octreePrev = prev;
            }

            PathGraphNode newNode = PathGraphNode.FromOctreeNode(next);
            if (next.hasNonNullChildren)
                queue.Enqueue(newNode);

            Edge newEdge = new(prevPathGraphNode, newNode, Dist3D(prevPathGraphNode, newNode));
            prevPathGraphNode?.Edges.Add(newEdge);

            ret.NodeSet.Add(newNode);
            ret.EdgeSet.Add(newEdge);

            ret.NodeCount++;
            ret.EdgeCount++;
        });

        ret.Root = pathRoot;
        ret.Initialized = true;

        // nodeSet should be of size n = 1337
        // edgeSet of size n - 1 = 1336
        return ret;
    }

    public void ConnectGraph()
    {
        Dictionary<PathGraphNode, List<Edge>> edgeBuffer = new() { { Root, new List<Edge>() } };

        // connect parents
        Bfs(Root, (prev, next, _, _) =>
        {
            // only have to add to edge buffer in this first BFS
            edgeBuffer.Add(next, new List<Edge>());

            edgeBuffer[next].Add(new Edge(next, prev, Dist3D(next, prev)));
        });

        Bfs(Root, (prev, next, notVisited, i) =>
        {
            if (i < notVisited.Count - 1)
            {
                if (i == 0)
                {
                    edgeBuffer[next].Add(new Edge(next, notVisited[^1], Dist3D(next, notVisited[^1])));
                }

                // "left" and "right" have little meaning in 3D space here, but this helps with intuition for the 2D representation of the graph
                PathGraphNode rightSibling = notVisited[i + 1];
                edgeBuffer[next].Add(new Edge(next, rightSibling, Dist3D(next, rightSibling)));
            }

            if (i > 0)
            {
                if (i == notVisited.Count - 1)
                {
                    edgeBuffer[next].Add(new Edge(next, notVisited[0], Dist3D(next, notVisited[0])));
                }

                PathGraphNode leftSibling = notVisited[i - 1];
                edgeBuffer[next].Add(new Edge(next, leftSibling, Dist3D(next, leftSibling)));
            }
        });

        // add buffered edgeSet
        foreach (KeyValuePair<PathGraphNode, List<Edge>> item in edgeBuffer)
        {
            foreach (Edge edge in item.Value)
            {
                item.Key.Edges.Add(edge);
                EdgeSet.Add(edge);
                EdgeCount++;
            }
        }
    }
}
