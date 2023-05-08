using System;
using System.Collections.Generic;
using System.Linq;
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

        int visitedCount = 0;
        while (queue.Count > 0)
        {
            PathGraphNode prev = queue.Dequeue();

            List<PathGraphNode> notVisited = prev.edges.Where(edge => !visited.Contains(edge.node)).Select(e => e.node).ToList();
            for (int i = 0; i < notVisited.Count; i++)
            {
                visitedCount++;
                queue.Enqueue(notVisited[i]);
                visitFunc(prev, notVisited[i], notVisited, i);
            }

            visited.Add(prev);
        }
    }

    /// <summary>
    /// returns the distance between the center points of two given nodes
    /// </summary>
    /// <param name="n1">The first node</param>
    /// <param name="n2">The second node</param>
    /// <returns></returns>
    public static double Dist3D(PathGraphNode n1, PathGraphNode n2)
    {
        Vector3 n1Center = n1.bounds.center;
        Vector3 n2Center = n2.bounds.center;

        return Math.Sqrt(
            Math.Pow(n1Center.x - n2Center.x, 2)
            +
            Math.Pow(n1Center.y - n2Center.y, 2)
            +
            Math.Pow(n1Center.z - n2Center.z, 2)
        );
    }
}
