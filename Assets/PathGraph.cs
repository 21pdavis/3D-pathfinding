using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public class PathGraph
{
    public PathGraphNode root { private set; get; }
    public bool initialized { private set; get; }

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
            //if (next.children.All(n => n != null))
            if (next.hasNonNullChildren)
                queue.Enqueue(newNode);
            prevPathGraphNode.neighbors.Add(newNode);
        });

        ret.root = pathRoot;
        ret.initialized = true;
        return ret;
    }

    public void ConnectGraph()
    {
        // connect parents
        Algorithms.BFS(root, (prev, next, i) =>
        {
            next.neighbors.Add(prev);
        });

        // connect immediate siblings
        Algorithms.BFS(root, (prev, next, i) =>
        {

        });
    }
}
