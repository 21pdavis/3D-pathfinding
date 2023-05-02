using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

internal class Algorithms
{
    // need to prevent from revisiting nodes
    public static void BFS(OctreeNode root, Action<OctreeNode, OctreeNode> visitFunc)
    {
        Queue<OctreeNode> queue = new();
        queue.Enqueue(root);
        HashSet<OctreeNode> visited = new();

        while (queue.Count > 0)
        {
            OctreeNode prev = queue.Dequeue();

            foreach (OctreeNode node in prev.children.Where(node => node != null))
            {
                if (visited.Contains(node)) continue;
                queue.Enqueue(node);
                visitFunc(prev, node);
            }

            visited.Add(prev);
        }
    }

    public static void BFS(PathGraphNode root, Action<PathGraphNode, PathGraphNode> visitFunc)
    {
        Queue<PathGraphNode> queue = new();
        queue.Enqueue(root);
        HashSet<PathGraphNode> visited = new();

        while (queue.Count > 0)
        {
            PathGraphNode prev = queue.Dequeue();

            foreach (PathGraphNode node in prev.neighbors)
            {
                if (visited.Contains(node)) continue;
                queue.Enqueue(node);
                visitFunc(prev, node);
            }

            visited.Add(prev);
        }
    }
}
