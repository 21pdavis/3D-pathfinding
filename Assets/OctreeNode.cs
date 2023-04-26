using System.Collections.Generic;
using UnityEngine;

using static Functional;

public class OctreeNode
{
    // neighbors was originally "children", but moving away from octree representation
    //public List<OctreeNode> neighbors { get; private set; }
    public OctreeNode[] neighbors;
    public Bounds nodeBounds { get; }

    Bounds[] childBounds;
    float minSize;

    public bool containsPather = false;

    public OctreeNode(Bounds nodeBounds, float minSize)
    {
        neighbors = new OctreeNode[8];


        this.nodeBounds = nodeBounds;
        this.minSize = minSize;

        // This value represents one fourth of any given side of the cube xx------ 
        float quarter = nodeBounds.size.y / 4.0f;

        // this represents the size of any given side of any direct child (picture 8 small cubes packed into one larger one)
        float childLength = nodeBounds.size.y / 2;

        Vector3 childSize = new Vector3(childLength, childLength, childLength);

        // This is how you init an array in C#
        childBounds = new Bounds[8];

        // Bounds constructor takes in Vector3 center and Vector3 size
        // Just defining the boundaries of where these nodes could possibly be, *not* creating them yet
        // TODO: Make this prettier?
        childBounds[0] = new Bounds(nodeBounds.center + new Vector3(-quarter, -quarter, -quarter), childSize);
        childBounds[1] = new Bounds(nodeBounds.center + new Vector3(-quarter, -quarter, quarter), childSize);
        childBounds[2] = new Bounds(nodeBounds.center + new Vector3(-quarter, quarter, -quarter), childSize);
        childBounds[3] = new Bounds(nodeBounds.center + new Vector3(-quarter, quarter, quarter), childSize);
        childBounds[4] = new Bounds(nodeBounds.center + new Vector3(quarter, -quarter, -quarter), childSize);
        childBounds[5] = new Bounds(nodeBounds.center + new Vector3(quarter, -quarter, quarter), childSize);
        childBounds[6] = new Bounds(nodeBounds.center + new Vector3(quarter, quarter, -quarter), childSize);
        childBounds[7] = new Bounds(nodeBounds.center + new Vector3(quarter, quarter, quarter), childSize);
    }

    public void AddObject(GameObject wo)
    {
        DivideAndAdd(wo);
    }

    public void DivideAndAdd(GameObject wo)
    {
        if (nodeBounds.size.y <= minSize)
        {
            return;
        }

        // set to true if we're about to divide space further
        for (int i = 0; i < 8; i++)
        {
            if (neighbors[i] == null)
            {
                // couldn't we just move this down to the lower if statement? Maybe we need to be able to access all 8 children if there are *any* children
                neighbors[i] = new OctreeNode(childBounds[i], minSize);
            }
            // GetComponent method seems to be roughly equivalent to an accessor method, where the type passed into the Generic is what is retrieved
            if (childBounds[i].Intersects(wo.GetComponent<Collider>().bounds))
            {
                neighbors[i].DivideAndAdd(wo);
            }
        }
    }

    public void Draw()
    {
        DrawWireCubeWithColor(nodeBounds, containsPather ? Color.yellow : Color.green);
        if (neighbors != null)
        {
            for (int i = 0; i < 8; i++)
            {
                if (neighbors[i] != null)
                {
                    neighbors[i].Draw();
                }
            }
        }
    }

    //public void DrawTraverse()
    //{
    //    // need to recurse down from the root using BFS, right? Kinda.
    //    if (children != null)
    //    {
    //        foreach (OctreeNode child in children)
    //        {
    //            // shouldn't need to use Functional DrawLine here - just make sure to call DrawTraverse with Functional.DrawWithColor
    //            Gizmos.DrawLine(nodeBounds.center, child.nodeBounds.center);
    //            child?.DrawTraverse();
    //        }
    //    }
    //}
}
