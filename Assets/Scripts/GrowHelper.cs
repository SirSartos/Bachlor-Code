using UnityEngine;
using System.Collections.Generic;


public class GrowHelper : MonoBehaviour 
{
    public float minTwigWidth = 0.01f;
    public float maxTwigWidth = 0.3f;
    public float widthScale = 1.4f;

    public GameObject Stamm;
    public GameObject SpacePoint;

    public List<GameObject> generateTreeObjects(TreeNode current, Transform parent, int infinitbreak)
    {
        List<GameObject> treeObjects = new List<GameObject>();
        genTree(current, parent, infinitbreak, treeObjects);
        return treeObjects;
    }

    private List<GameObject> genTree(TreeNode current, Transform parent, int infinitbreak, List<GameObject> add, TreeNode start = null)
    {
        if (start == null)
        {
            start = current;
        }
        if (current.next != null && infinitbreak > 0)
        {
            foreach (TreeNode node in current.next)
            {
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, node.position - current.position);
                GameObject tempStamm = Instantiate(Stamm, (current.position + node.position) / 2, rotation);
                tempStamm.transform.parent = parent;
                float scaleY = Vector3.Distance(current.position, node.position) / 2;
                float widht = (minTwigWidth + (maxTwigWidth - minTwigWidth) * Mathf.Pow(((node.LongestTwig() * 1f) / start.LongestTwig()), widthScale));
                tempStamm.transform.localScale = new Vector3(widht, scaleY, widht);
                add.Add(tempStamm);
                add = genTree(node, parent, --infinitbreak, add, start);
            }
        }
        return add;
    }

    public List<GameObject> renderTreePoints(TreeNode start)
    {
        List<GameObject> spacePoints = new List<GameObject>();
        renTreePoint(start, spacePoints);
        return spacePoints;
    }

    private List<GameObject> renTreePoint(TreeNode start, List<GameObject> add)
    {
        add.Add(Instantiate(SpacePoint, start.position, Quaternion.identity));
        foreach (TreeNode node in start.next)
        {
            add = renTreePoint(node, add);
        }
        return add; 
    }

    //Static methods

    public static Vector3 randomVec3(float min, float max)
    {
        return new Vector3(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max));
    }

    public static void printArray(Vector3[] array)
    {
        for (int i = 0; i < array.Length;i++)
        {
            Debug.Log(array[i]);
        }
    }

    public static void printArray(float[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            Debug.Log(array[i]);
        }
    }
}
