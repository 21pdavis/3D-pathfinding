﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

internal static class Algorithms
{
    // need to prevent from revisiting nodeSet
    public static void Bfs(OctreeNode root, Action<OctreeNode, OctreeNode> visitFunc)
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

    public static void Bfs(PathGraphNode root, Action<PathGraphNode, PathGraphNode, List<PathGraphNode>, int> visitFunc)
    {
        Queue<PathGraphNode> queue = new();
        queue.Enqueue(root);
        HashSet<PathGraphNode> visited = new();

        int visitedCount = 0;
        while (queue.Count > 0)
        {
            PathGraphNode prev = queue.Dequeue();

            List<PathGraphNode> notVisited = prev.Edges.Where(edge => !visited.Contains(edge.To)).Select(e => e.To).ToList();
            for (int i = 0; i < notVisited.Count; i++)
            {
                visitedCount++;
                queue.Enqueue(notVisited[i]);
                visitFunc(prev, notVisited[i], notVisited, i);
            }

            visited.Add(prev);
        }
    }

    public static IEnumerator CoroutineBfs(PathGraphNode root, Action<PathGraphNode, PathGraphNode, List<PathGraphNode>, int> visitFunc)
    {
        float delaySecs = 0f;
        
        Queue<PathGraphNode> queue = new();
        queue.Enqueue(root);
        HashSet<PathGraphNode> visited = new();

        while (queue.Count > 0)
        {
            PathGraphNode prev = queue.Dequeue();

            List<PathGraphNode> notVisited = prev.Edges.Where(edge => !visited.Contains(edge.To)).Select(e => e.To).ToList();
            for (int i = 0; i < notVisited.Count; i++)
            {
                queue.Enqueue(notVisited[i]);
                visitFunc(prev, notVisited[i], notVisited, i);
            }

            visited.Add(prev);

            yield return new WaitForSeconds(delaySecs);
        }
    }

    // Should I draw inside this algorithm or use a visitFunc? Probably safe to draw inside, but we'll see
    // Use a priority queue for O(mlogn) runtime because m = O(mn)
    public static IEnumerator Dijkstra(PathGraph graph, PathGraphNode source=null)
    {
        // if no source specified, set to root
        source ??= graph.Root;

        // initialize S and d
        HashSet<PathGraphNode> explored = new() { source };
        Dictionary<PathGraphNode, double> dist = new() { { source, 0 } };

        /*
         * Priority Queue implemented with a quaternary heap (https://en.wikipedia.org/wiki/D-ary_heap), sourced from https://stackoverflow.com/a/73430119/12228952
         *
         * Quaternary heap is a heap where each to has d=4 children. For a quaternary heap with n nodeSet, it is better for Dijkstra's because it has a practically more efficient
         * time to decrease priority (O(dlog(n)) = O(log_4(n))), but a less efficient DeleteMin operation (O(dlog_d(n)) = O(log_4(n))). These bounds are actually the same asymptotically, though.
         * It is more common in Dijkstra's to do decrease priority, which is why using this data structure makes sense.
         */
        // TODO: MAKE SURE TO CHECK FOR REPEATED CHECKS, DON'T WANT TO RE-SET A NODE'S DIST VALUE
        PriorityQueue<Edge, double> prio = new();

        foreach (Edge edge in source.Edges)
        {
            // enqueue with weight across cut (S, V - S)
            prio.Enqueue(edge, edge.Weight);
        }

        int repeatCount = 0;
        // Overall O(mlogn)
        while (explored.Count < graph.NodeCount) // O(n)
        {
            // Select a to v not in S with at least one edge from S for which d'(v) = min_{e=(u,v):u in S}(dist[u] + e.weight) is as small as possible
            double distanceAcrossCut = prio.PriorityPeek(); // O(1)
            Edge nextEdge = prio.Dequeue(); // O(4logn/log4) = O(logn)

            // checking to see how redundant this is...
            // The answer was fairly redundant, repeatCount came out to be ~3000, which is honestly less than I expected, though still not great
            if (explored.Contains(nextEdge.To))
            {
                repeatCount++;
                continue;
            }

            /*
             * Visit Steps:
             * 1. Add all nodeSet to which next has an edge to prio queue
             * 2. Set dist[next.to] = dist[next.from] + distanceAcrossCut
             * 3. Add next.to to explored
             */

            foreach (Edge edge in nextEdge.To.Edges)
            {
                prio.Enqueue(edge, edge.Weight);
            }

            dist[nextEdge.To] = dist[nextEdge.From] + distanceAcrossCut;

            // mark this node explored, no need to look at it again by correctness of Dijkstra on positive edge weight directed graph
            explored.Add(nextEdge.To);

            // distance is set, this is shortest path, so draw line
            graph.DijkstraLines.Add((nextEdge.From.Bounds.center, nextEdge.To.Bounds.center));

            yield return new WaitForSeconds(0.02f);
        }
        Debug.Log($"Made it to end of Dijkstra's");
    }

    /// <summary>
    /// DP algorithm to compute shortest paths. Slightly different to algorithm presented in textbook,
    /// modified to be single source all destination instead of single sink
    /// </summary>
    /// <param name="graph"></param>
    public static IEnumerator BellmanFord(PathGraph graph, PathGraphNode source=null)
    {
        // if no source specified, set to root
        source ??= graph.Root;

        // get set of all nodeSet to which we assign distances from source
        Dictionary<PathGraphNode, double> dist = new();
        Dictionary<PathGraphNode, PathGraphNode> pred = new() { { source, null } };

        foreach (PathGraphNode node in graph.NodeSet)
        {
            dist.Add(node, int.MaxValue);
        }

        dist[source] = 0;

        // edge relaxation
        // Interesting observation: BellmanFord grows outwardly closer to how BFS works *because of the order of the edge set*,
        // but Dijkstra's is more unpredictable. It is not necessarily like DFS, but it will just grab the next closest edge
        // The book says normal BF is 
        for (int i = 0; i < dist.Count - 1; i++)
        {
            foreach (Edge edge in graph.EdgeSet)
            {
                if (dist[edge.From] + edge.Weight < dist[edge.To])
                {
                    dist[edge.To] = dist[edge.From] + edge.Weight;
                    pred[edge.To] = edge.From;
                    graph.BellmanFordLines.Add((edge.From.Bounds.center, edge.To.Bounds.center));
                }
                yield return new WaitForSeconds(0.02f);
            }

        }

        // observation: edgeSet.Count = 4*(nodeSet.Count - 1), not sure if that's noteworthy 
        Debug.Log($"dist.Count is {dist.Count}");
    }

    /// <summary>
    /// Two related optimizations made in this implementation:
    /// 1. Tracking which nodes are updated at each step
    /// 2. Breaking out early if no nodes were updated in the last step
    /// The second can only be done because we are doing the first, so it has somewhat of a compounding positive effect on the run time
    /// </summary>
    /// <param name="graph"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    /// TODO: Implement using Queue? That's what wikipedia does...
    public static IEnumerator BellmanFordMoore(PathGraph graph, PathGraphNode source = null)
    {
        // if no source specified, set to root
        source ??= graph.Root;

        // get set of all nodeSet to which we assign distances from source
        Dictionary<PathGraphNode, double> dist = new();
        Dictionary<PathGraphNode, PathGraphNode> pred = new() { { source, null } };

        foreach (PathGraphNode node in graph.NodeSet)
        {
            dist.Add(node, int.MaxValue);
        }

        dist[source] = 0;

        // Q: Why was the original BF running slowly?
        // A: Because we were checking every single edge at every execution. A better way is to track a cutset of edges that go across the cut
        HashSet<Edge> edgesFromUpdatedNodes = new();

        // start with edges out of node - these are the only edges that will be updated in the first iteration
        foreach (Edge edge in source.Edges)
        {
            edgesFromUpdatedNodes.Add(edge);
        }

        int updatedCountMax = int.MinValue;
        int updatedCountSum = 0;
        int numberOfIterations = 0;
        // All the same as unoptimized up until this point
        for (int i = 0; i < dist.Count - 1; i++)
        {
            numberOfIterations = i;
            if (edgesFromUpdatedNodes.Count == 0)
            {
                Debug.Log($"Optimized Bellman-Ford broke early at iteration {i}");
                break;
            }

            updatedCountSum += edgesFromUpdatedNodes.Count;
            if (edgesFromUpdatedNodes.Count > updatedCountMax)
            {
                updatedCountMax = edgesFromUpdatedNodes.Count;
            }

            HashSet<PathGraphNode> updatedNodes = new();
            // here's where it starts to be a little different - we update cutSet progressively
            foreach (Edge edge in edgesFromUpdatedNodes)
            {
                if (dist[edge.From] + edge.Weight < dist[edge.To])
                {
                    dist[edge.To] = dist[edge.From] + edge.Weight;
                    pred[edge.To] = edge.From;
                    graph.BellmanFordMooreLines.Add((edge.From.Bounds.center, edge.To.Bounds.center));

                    updatedNodes.Add(edge.To);
                }
            }

            // Is this actually O(mn)?
            foreach (PathGraphNode updatedNode in updatedNodes)
            {
                foreach (Edge edgeFromUpdatedNode in updatedNode.Edges) 
                {
                    edgesFromUpdatedNodes.Add(edgeFromUpdatedNode);
                }
            }

            edgesFromUpdatedNodes = edgesFromUpdatedNodes.Where(e => updatedNodes.Contains(e.From)).ToHashSet();

            yield return new WaitForSeconds(0.02f);
        }

        Debug.Log($"Bellman-Ford-Moore Finished after {numberOfIterations} iterations");

        int updatedSumFlooredAverage = updatedCountSum / numberOfIterations;
        Debug.Log($"Average number of edges checked per iteration was {updatedSumFlooredAverage}, which is an improvement over {graph.EdgeCount}");
        Debug.Log($"Maximum number of edges checked in some iteration was {updatedCountMax}, which is an improvement over {graph.EdgeCount}");
    }

    /// <summary>
    /// returns the distance between the center points of two given nodeSet
    /// </summary>
    /// <param name="n1">The first to</param>
    /// <param name="n2">The second to</param>
    /// <returns></returns>
    public static double Dist3D(PathGraphNode n1, PathGraphNode n2)
    {
        Vector3 n1Center = n1.Bounds.center;
        Vector3 n2Center = n2.Bounds.center;

        return Math.Sqrt(
            Math.Pow(n1Center.x - n2Center.x, 2)
            +
            Math.Pow(n1Center.y - n2Center.y, 2)
            +
            Math.Pow(n1Center.z - n2Center.z, 2)
        );
    }
}
