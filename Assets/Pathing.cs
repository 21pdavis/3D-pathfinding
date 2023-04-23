using UnityEngine;

[System.Serializable]
public class Pathing : MonoBehaviour
{
    /*
     So here's what I want to do here. I want to make it so this script can find the closest octree node to the object to which it is attached.
     To do that, I need to help it find the Octree GameObject
     */

    private GameObject octreeGameObject;
    private CreateOctree createOctreeScript;
    private Octree octree;

    private void Start()
    {
        octreeGameObject = GameObject.Find("Octree");
        createOctreeScript = octreeGameObject.GetComponent<CreateOctree>();
    }

    private void FixedUpdate()
    {
        if (octree != null && octree.initialized == false)
        {
            // run a graph search (BFS) to find rootNode closest to Sphere
            OctreeNode patherNode = FindNodeOfPather();
            // run Dijkstra's from this node to find the minimum path
        }
        else if (octree == null)
        {
            octree = createOctreeScript.octree;
        }
    }

    private void OnDrawGizmos()
    {
        // Here is where we will draw the result of the Dijkstra's run done in FixedUpdate
    }

    /// <summary>
    /// Get the Octree node closest to the current GameObject
    /// </summary>
    /// <returns>
    /// A reference to an OctreeNode with the minimal center-to-center distance between its bounds
    /// and the current GameObject
    /// </returns>
    private OctreeNode FindNodeOfPather()
    {
        return null;
    }
}
