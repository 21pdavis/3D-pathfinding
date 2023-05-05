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

            foreach (OctreeNode node in prev.children.Where(node => node != null && !visited.Contains(node)))
            {
                queue.Enqueue(node);
                visitFunc(prev, node);
            }

            visited.Add(prev);
        }
    }

    public static void BFS(PathGraphNode root, Action<PathGraphNode, PathGraphNode, List<PathGraphNode>, int> visitFunc)
    {
        Queue<PathGraphNode> queue = new();
        queue.Enqueue(root);
        HashSet<PathGraphNode> visited = new();

        while (queue.Count > 0)
        {
            PathGraphNode prev = queue.Dequeue();

            // index for 
            List<PathGraphNode> notVisited = prev.neighbors.Where(node => !visited.Contains(node)).ToList();
            for (int i = 0; i < notVisited.Count; i++)
            {
                queue.Enqueue(notVisited[i]);
                visitFunc(prev, notVisited[i], notVisited, i);
            }

            visited.Add(prev);
        }
    }
}
