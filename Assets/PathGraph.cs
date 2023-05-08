using System.Collections.Generic;

using static Algorithms;

public class PathGraph
{
    public PathGraphNode root { private set; get; }
    public bool initialized { private set; get; }
    public int nodeCount { private set; get; }
    public int edgeCount { private set; get; }

    // initialize PathGraph using a level-order traversal of the given octree
    public static PathGraph FromOctree(Octree octree)
    {
        PathGraph ret = new();

        Queue<PathGraphNode> queue = new();
        PathGraphNode prevPathGraphNode = null;
        OctreeNode pathPrev = null;
        PathGraphNode pathRoot = null;

        BFS(octree.root, (prev, next) =>
        {
            if (pathPrev != prev)
            {
                if (prev == octree.root)
                {
                    pathRoot = PathGraphNode.FromOctreeNode(prev);
                    queue.Enqueue(pathRoot);
                }
                prevPathGraphNode = queue.Dequeue();
                pathPrev = prev;
            }

            PathGraphNode newNode = PathGraphNode.FromOctreeNode(next);
            if (next.hasNonNullChildren)
                queue.Enqueue(newNode);
            //prevPathGraphNode.edges.Add(newNode);
            prevPathGraphNode?.edges.Add(new Edge(newNode, Dist3D(prevPathGraphNode, newNode)));
            ret.nodeCount++;
            ret.edgeCount++;
        });

        ret.root = pathRoot;
        ret.initialized = true;
        return ret;
    }

    public void ConnectGraph()
    {
        // connect parents
        BFS(root, (prev, next, _, _) =>
        {
            //next.edges.Add(prev);
            next.edges.Add(new Edge(prev, Dist3D(next, prev)));
            edgeCount++;
        });

        Dictionary<PathGraphNode, List<Edge>> edgeBuffer = new();
        BFS(root, (prev, next, notVisited, i) =>
        {
            // we can assume this only happens once per node
            edgeBuffer.Add(next, new List<Edge>());

            if (i < notVisited.Count - 1)
            {
                //edgeBuffer[next].Add(notVisited[i + 1]);
                // "left" and "right" have little meaning in 3D space here, but this helps with intuition for the 2D representation of the graph
                PathGraphNode rightSibling = notVisited[i + 1];
                edgeBuffer[next].Add(new Edge(rightSibling, Dist3D(next, rightSibling)));
            }

            if (i > 0)
            {
                //edgeBuffer[next].Add(notVisited[i - 1]);
                PathGraphNode leftSibling = notVisited[i - 1];
                edgeBuffer[next].Add(new Edge(leftSibling, Dist3D(next, leftSibling)));
            }
        });

        // add buffered edges
        foreach (KeyValuePair<PathGraphNode, List<Edge>> item in edgeBuffer)
        {
            foreach (Edge edge in item.Value)
            {
                //node.edges.Add(node);
                item.Key.edges.Add(edge);
                edgeCount++;
            }
        }
    }
}
