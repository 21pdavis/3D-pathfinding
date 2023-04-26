using UnityEngine;

// TODO: Rename to SubdivGraph?
public class Octree
{
    public OctreeNode root;
    public bool initialized = false;

    public Octree(GameObject[] worldObjects, float minNodeSize)
    {
        // Bounds is an Axis-Aligned Bounding Box (AABB) - edges are aligned to one of x, y, z axes
        Bounds bounds = new Bounds();

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

        root = new OctreeNode(bounds, minNodeSize);

        AddObjects(worldObjects);
        initialized = true;
    }

    public void AddObjects(GameObject[] worldObjects)
    {
        // we do a pass of subdivision for each object in the scene
        foreach (GameObject wo in worldObjects)
        {
            root.AddObject(wo);
        }
    }
}
