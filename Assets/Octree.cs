using UnityEngine;

public class Octree
{
    public OctreeNode Root { private set; get; }
    public bool initialized;

    public Octree(GameObject[] worldObjects, float minNodeSize)
    {
        // Bounds is an Axis-Aligned Bounding Box (AABB) - edgeSet are aligned to one of x, y, z axes
        Bounds bounds = new();

        // bounds start off at (0, 0, 0) with extents (0, 0, 0), but this for loop recenters it
        // In current scene arrangement, the center is at (-12.02, 2.34, -1.56)
        foreach (GameObject wo in worldObjects)
        {
            // grow the bounds of our local bounds to encapsulate the total bounds of all world objects passed in (create big, overall bounding box)
            bounds.Encapsulate(wo.GetComponent<Collider>().bounds);
        }

        // find largest of the x, y, z dimension - make that the x, y, and z dimension of our cube
        float maxSize = Mathf.Max(new float[] { bounds.size.x, bounds.size.y, bounds.size.z });
        // put that maxSize into a new Vector3
        Vector3 sizeVector = new Vector3(maxSize, maxSize, maxSize);

        bounds.SetMinMax(bounds.center - sizeVector / 2, bounds.center + sizeVector / 2);

        Root = new OctreeNode(bounds, minNodeSize);

        AddObjects(worldObjects);
        initialized = true;
    }

    public void AddObjects(GameObject[] worldObjects)
    {
        // we do a pass of subdivision for each object in the scene
        foreach (GameObject wo in worldObjects)
        {
            Root.AddObject(wo);
        }
    }

    /// <summary>
    /// Iterate through the graph to connect all siblings (nodeSet on the same layer)
    /// to each other as well as create child-to-parent edgeSet
    /// 
    /// Should be executed only once if possible
    /// </summary>
    public void ConnectGraph()
    {

    }
}
