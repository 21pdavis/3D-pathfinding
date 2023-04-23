using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Functional;

[System.Serializable]
public class CreateOctree : MonoBehaviour
{
    public GameObject[] worldObjects;
    public int nodeMinSize = 1;
    public Octree octree;
    public bool showSearch;
    public bool showColliders;

    // Start is called before the first frame update
    void Start()
    {
        // Question: How is worldObjects populated? A: in unity
        octree = new Octree(worldObjects, nodeMinSize);
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            DrawWithColor(octree.root.Draw, Color.green);

            if (showSearch)
            {
                DrawWithColor(octree.root.DrawTraverse, Color.red);
            }

            if (showColliders)
            {
                Gizmos.color = Color.red;
                foreach (GameObject obj in worldObjects)
                {
                    Bounds objBounds = obj.GetComponent<Collider>().bounds;
                    DrawWireCubeWithColor(objBounds, Color.red);
                }
            }
        }
    }
}
