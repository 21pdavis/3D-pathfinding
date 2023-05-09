using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;

using static Functional;

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

    // Should I draw inside this algorithm or use a visitFunc? Probably safe to draw inside, but we'll see
    // Use a priority queue for O(mlogn) runtime because m = O(mn)
    // TODO: Make push-based?
    public static void Dijkstra(PathGraph graph)
    {
        // initialize S and d
        HashSet<PathGraphNode> explored = new() { graph.root };
        Dictionary<PathGraphNode, double> dist = new() { { graph.root, 0 } };

        // Priority Queue implemented with a quaternary heap (https://en.wikipedia.org/wiki/D-ary_heap), sourced from https://stackoverflow.com/a/73430119/12228952
        /*
         * Quaternary heap is a heap where each node has d=4 children. For a quaternary heap with n nodes, it is better for Dijkstra's because it has a practically more efficient
         * time to decrease priority (O(dlog(n)) = O(log_4(n))), but a less efficient DeleteMin operation (O(dlog_d(n)) = O(log_4(n))). These bounds are actually the same asymptotically, though.
         * It is more common in Dijkstra's to do decrease priority, which is why using this data structure makes sense.
         */
        // TODO: MAKE SURE TO CHECK FOR REPEATED CHECKS, DON'T WANT TO RE-SET A NODE'S DIST VALUE
        PriorityQueue<(PathGraphNode from, PathGraphNode to), double> prio = new();

        foreach (Edge edge in graph.root.edges)
        {
            // enqueue with weight across cut (S, V - S)
            prio.Enqueue((graph.root, edge.node), edge.weight);
        }

        // TODO: Maybe fix BFS not properly visiting node
        // Get set of all nodes Swe
        HashSet<PathGraphNode> allNodes = new() { graph.root };
        BFS(graph.root, (prev, next, _, _) =>
        {
            allNodes.Add(next);
        });

        int repeatCount = 0;
        // Not positive that this condition works
        while (explored.Count < graph.nodeCount)
        {
            // Select a node v not in S with at least one edge from S for which d'(v) = min_{e=(u,v):u in S}(dist[u] + e.weight) is as small as possible
            double distanceAcrossCut = prio.PriorityPeek();
            (PathGraphNode from, PathGraphNode to) next = prio.Dequeue();
            // checking to see how redundant this is...
            if (explored.Contains(next.to))
            {
                repeatCount++;
                continue;
            }

            /*
             * Visit Steps:
             * 1. Add all nodes to which next has an edge to prio queue
             * 2. Set dist[next.to] = dist[next.from] + distanceAcrossCut
             * 3. Add next.to to explored
             */

            foreach (Edge edge in next.to.edges)
            {
                prio.Enqueue((next.to, edge.node), edge.weight);
            }

            dist[next.to] = dist[next.from] + distanceAcrossCut;

            // distance is set, this is shortest path, so draw line
            DrawLineWithColor(next.from.bounds.center, next.to.bounds.center, Color.yellow);

            explored.Add(next.to);
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
