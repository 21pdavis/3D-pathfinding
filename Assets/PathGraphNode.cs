using System.Collections.Generic;
using UnityEngine;

using static Functional;

public class PathGraphNode
{
    public List<Edge> Edges { set; get; }
    public bool containsPather;
    public Bounds Bounds { private set; get; }

    public static PathGraphNode FromOctreeNode(OctreeNode node)
    {
        PathGraphNode ret = new()
        {
            Edges = new List<Edge>(),
            containsPather = false,
            Bounds = new Bounds(node.Bounds.center, node.Bounds.size)
        };

        return ret;
    }

    public void Draw(Color color)
    {
        Algorithms.Bfs(this, (prev, next, i, len) =>
        {
            DrawWireCubeWithColor(next.Bounds, next.containsPather ? Color.yellow : Color.green);
        });
    }
}
