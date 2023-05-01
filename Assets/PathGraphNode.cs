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
        PathGraphNode ret = new();

        ret.neighbors = new List<PathGraphNode>();
        ret.containsPather = false;
        ret.bounds = new Bounds(node.bounds.center, node.bounds.size);

        return ret;
    }

    public void Draw()
    {
        DrawWireCubeWithColor(bounds, containsPather ? Color.yellow : Color.green);

        foreach (PathGraphNode node in neighbors)
        {
            node.Draw();
        }
    }
}
