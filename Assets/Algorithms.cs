using System.Collections.Generic;
using UnityEngine;

internal class Algorithms
{
    public static void BFS(OctreeNode root, bool draw=true)
    {
        Queue<OctreeNode> queue = new();
        queue.Enqueue(root);

        OctreeNode prev = null;
        while (queue.Count > 0)
        {
            prev = queue.Dequeue();

            foreach (OctreeNode node in prev.neighbors)
            {
                if (node != null)
                {
                    queue.Enqueue(node);
                    if (draw) Gizmos.DrawLine(prev.nodeBounds.center, node.nodeBounds.center);
                }
            }
        }
    }

}
