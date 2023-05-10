using System.Collections.Generic;
using Unity.VisualScripting;
using static Algorithms;

public class PathGraph
{
    public PathGraphNode root { private set; get; }
    public HashSet<PathGraphNode> nodeSet { private set; get; } = new();
    public HashSet<Edge> edgeSet { private set; get; } = new();
    public bool initialized { private set; get; }
    public int nodeCount { private set; get; } = 1; // in the current version of the graph, this number should come out to 1337
    public int edgeCount { private set; get; }

    // initialize PathGraph using a level-order traversal of the given octree
    public static PathGraph FromOctree(Octree octree)
    {
        PathGraph ret = new();

        Queue<PathGraphNode> queue = new();
        PathGraphNode prevPathGraphNode = null;
        OctreeNode octreePrev = null;
        PathGraphNode pathRoot = null;

        BFS(octree.root, (prev, next) =>
        {
            if (octreePrev != prev)
            {
                if (prev == octree.root)
                {
                    pathRoot = PathGraphNode.FromOctreeNode(prev);
                    ret.nodeSet.Add(pathRoot);
                    queue.Enqueue(pathRoot);
                }
                prevPathGraphNode = queue.Dequeue();
                octreePrev = prev;
            }

            PathGraphNode newNode = PathGraphNode.FromOctreeNode(next);
            if (next.hasNonNullChildren)
                queue.Enqueue(newNode);

            Edge newEdge = new(prevPathGraphNode, newNode, Dist3D(prevPathGraphNode, newNode));
            prevPathGraphNode?.edges.Add(newEdge);

            ret.nodeSet.Add(newNode);
            ret.edgeSet.Add(newEdge);

            ret.nodeCount++;
            ret.edgeCount++;
        });

        ret.root = pathRoot;
        ret.initialized = true;

        // nodeSet should be of size n = 1337
        // edgeSet of size n - 1 = 1336
        return ret;
    }

    public void ConnectGraph()
    {
        Dictionary<PathGraphNode, List<Edge>> edgeBuffer = new() { { root, new List<Edge>() } };

        // connect parents
        BFS(root, (prev, next, _, _) =>
        {
            // only have to add to edge buffer in this first BFS
            edgeBuffer.Add(next, new List<Edge>());

            edgeBuffer[next].Add(new Edge(next, prev, Dist3D(next, prev)));
        });

        BFS(root, (prev, next, notVisited, i) =>
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
                item.Key.edges.Add(edge);
                edgeCount++;
            }
        }
    }
}
