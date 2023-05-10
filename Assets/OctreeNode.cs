using UnityEngine;

using static Functional;

public class OctreeNode
{
    // children was originally "children", but moving away from octree representation
    public OctreeNode[] children;
    public Bounds bounds { get; }

    private Bounds[] childBounds;
    private float minSize;

    public bool hasNonNullChildren = false;
    public bool containsPather = false;

    public OctreeNode(Bounds bounds, float minSize)
    {
        children = new OctreeNode[8];

        this.bounds = bounds;
        this.minSize = minSize;

        // This value represents one fourth of any given side of the cube xx------ 
        float quarter = bounds.size.y / 4.0f;

        // this represents the size of any given side of any direct child (picture 8 small cubes packed into one larger one)
        float childLength = bounds.size.y / 2;

        Vector3 childSize = new(childLength, childLength, childLength);

        // This is how you init an array in C#
        childBounds = new Bounds[8];

        // Bounds constructor takes in Vector3 center and Vector3 size
        // Just defining the boundaries of where these nodeSet could possibly be, *not* creating them yet
        // TODO: Make this prettier?
        childBounds[0] = new Bounds(bounds.center + new Vector3(-quarter, -quarter, -quarter), childSize);
        childBounds[1] = new Bounds(bounds.center + new Vector3(-quarter, -quarter, quarter), childSize);
        childBounds[2] = new Bounds(bounds.center + new Vector3(-quarter, quarter, -quarter), childSize);
        childBounds[3] = new Bounds(bounds.center + new Vector3(-quarter, quarter, quarter), childSize);
        childBounds[4] = new Bounds(bounds.center + new Vector3(quarter, -quarter, -quarter), childSize);
        childBounds[5] = new Bounds(bounds.center + new Vector3(quarter, -quarter, quarter), childSize);
        childBounds[6] = new Bounds(bounds.center + new Vector3(quarter, quarter, -quarter), childSize);
        childBounds[7] = new Bounds(bounds.center + new Vector3(quarter, quarter, quarter), childSize);
    }

    public void AddObject(GameObject wo)
    {
        DivideAndAdd(wo);
    }

    public void DivideAndAdd(GameObject wo)
    {
        if (bounds.size.y <= minSize)
        {
            return;
        }

        hasNonNullChildren = true;
        // set to true if we're about to divide space further
        for (int i = 0; i < 8; i++)
        {
            if (children[i] == null)
            {
                // couldn't we just move this down to the lower if statement? Maybe we need to be able to access all 8 children if there are *any* children
                children[i] = new OctreeNode(childBounds[i], minSize);
            }

            // GetComponent method seems to be roughly equivalent to an accessor method, where the type passed into the Generic is what is retrieved
            if (childBounds[i].Intersects(wo.GetComponent<Collider>().bounds))
            {
                children[i].DivideAndAdd(wo);
            }
        }

    }

    public void Draw()
    {
        DrawWireCubeWithColor(bounds, containsPather ? Color.yellow : Color.green);
        if (children != null)
        {
            for (int i = 0; i < 8; i++)
            {
                if (children[i] != null)
                {
                    children[i].Draw();
                }
            }
        }
    }
}
