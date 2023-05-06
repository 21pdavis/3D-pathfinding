using System.Collections.Generic;

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

        Algorithms.BFS(octree.root, (prev, next) =>
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
            prevPathGraphNode.neighbors.Add(newNode);
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
        Algorithms.BFS(root, (prev, next, _, _) =>
        {
            next.neighbors.Add(prev);
            edgeCount++;
        });

        Dictionary<PathGraphNode, List<PathGraphNode>> edgeBuffer = new();
        Algorithms.BFS(root, (prev, next, notVisited, i) =>
        {
            edgeBuffer.Add(next, new List<PathGraphNode>());

            if (i < notVisited.Count - 1)
            {
                edgeBuffer[next].Add(notVisited[i + 1]);
            }

            if (i > 0)
            {
                edgeBuffer[next].Add(notVisited[i - 1]);
            }
        });

        // add buffered edges
        foreach (KeyValuePair<PathGraphNode, List<PathGraphNode>> item in edgeBuffer)
        {
            foreach (PathGraphNode node in item.Value)
            {
                node.neighbors.Add(node);
                edgeCount++;
            }
        }
    }
}
