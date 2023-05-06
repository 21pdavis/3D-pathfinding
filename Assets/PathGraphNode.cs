using System.Collections.Generic;
using UnityEngine;
using static Functional;

public class PathGraphNode
{
    public List<PathGraphNode> neighbors { set; get; }
    public bool containsPather;
    public Bounds bounds { private set; get; }

    public static PathGraphNode FromOctreeNode(OctreeNode node)
    {
        PathGraphNode ret = new()
        {
            neighbors = new List<PathGraphNode>(),
            containsPather = false,
            bounds = new Bounds(node.bounds.center, node.bounds.size)
        };

        return ret;
    }

    public void Draw()
    {
        Algorithms.BFS(this, (prev, next, i, len) =>
        {
            DrawWireCubeWithColor(next.bounds, next.containsPather ? Color.yellow : Color.green);
        });
    }
}
