using System.Collections.Generic;
using UnityEngine;

using static Functional;

public class PathGraphNode
{
    public List<Edge> edges { set; get; }
    public bool containsPather;
    public Bounds bounds { private set; get; }

    public static PathGraphNode FromOctreeNode(OctreeNode node)
    {
        PathGraphNode ret = new()
        {
            edges = new List<Edge>(),
            containsPather = false,
            bounds = new Bounds(node.bounds.center, node.bounds.size)
        };

        return ret;
    }

    public void Draw(Color color)
    {
        Algorithms.BFS(this, (prev, next, i, len) =>
        {
            DrawWireCubeWithColor(next.bounds, next.containsPather ? Color.yellow : Color.green);
        });
    }
}
