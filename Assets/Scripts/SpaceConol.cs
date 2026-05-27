using UnityEngine;
using System.Collections.Generic;

public class SpaceConol : MonoBehaviour
{
    public GrowHelper growHelper;
    private List<GameObject> treeObjects = new List<GameObject>();
    private List<GameObject> spacePoints = new List<GameObject>();

    public int points = 1000;
    public Vector3[] space;
    public Vector3 shapeScale = new Vector3(2.5f, 2.5f, 2.5f);
    public float searchDistance = 1.12f;
    public float killDistance = 0.3f;
    public float spaceOffset = 3f;
    public int randSeed = 1314568912;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            foreach (GameObject go in treeObjects)
            {
                GameObject.Destroy(go);
            }
            foreach (GameObject go in spacePoints)
            {
                GameObject.Destroy(go);
            }
            makeTree();
        }else if (Input.GetKeyDown(KeyCode.T))
        {
            foreach(GameObject go in spacePoints)
            {
                go.SetActive(!go.activeSelf);
            }
        }
    }

    public void makeTree()
    {
        Random.InitState(randSeed);
        space = generateSpace(points);
        /* show Space Points*/
        TreeNode start = new TreeNode(gameObject.transform.position);
        generateTree(start, space);
        treeObjects = growHelper.generateTreeObjects(start, gameObject.transform, 10000);
        spacePoints = growHelper.renderTreePoints(start);
    }

    private void generateTree(TreeNode start, Vector3[] attractionPoints)
    {
        List<Vector3> attractionList = new List<Vector3>(attractionPoints);
        List<TreeNode> treeList = new List<TreeNode>();
        treeList.Add(start);
        int endlessBreak = 100;
        while (attractionList.Count > 0 && endlessBreak > 0)
        {
            endlessBreak--;
            if (endlessBreak == 0)
            {
                Debug.Log("wopsy i = 0. Attrlistcount = " + attractionList.Count);
            }
            // Part B
            Dictionary<TreeNode, List<Vector3>> treeNodes = new Dictionary<TreeNode, List<Vector3>>();
            foreach(Vector3 attr in attractionList)
            {
                TreeNode closest = treeList[0];
                float closestDist = killDistance * 10;
                foreach(TreeNode point in treeList)
                {
                    float dist = Vector3.Distance(point.position, attr);
                    if (dist < closestDist)
                    {
                        closestDist = Vector3.Distance(attr, closest.position);
                        closest = point;
                    }
                }
                if (closestDist <= searchDistance)
                {
                    if (!treeNodes.ContainsKey(closest))
                    {
                        treeNodes.Add(closest, new List<Vector3>());
                    }
                    treeNodes[closest].Add(attr);
                }
            }
            if (treeNodes.Keys.Count == 0)
            {
                TreeNode point = treeList[treeList.Count - 1];
                Vector3 closest = attractionList[0];
                foreach (Vector3 attr in attractionList)
                {
                    if (Vector3.Distance(point.position, attr) < Vector3.Distance(closest, point.position))
                    {
                        closest = attr;
                    }
                }
                Vector3 growDirection = (closest - point.position).normalized;
                start = instanceTreeBranch(point, growDirection, point.position);
                treeList.Add(start);
            }
            else
            {
                //Part C - E
                foreach (KeyValuePair<TreeNode, List<Vector3>> node in treeNodes)
                {
                    Vector3 growDirection = Vector3.zero;
                    foreach (Vector3 vec in node.Value)
                    {
                        growDirection += vec - node.Key.position;
                    }
                    if (growDirection != Vector3.zero)
                    {
                        growDirection.Normalize();
                        start = instanceTreeBranch(node.Key, growDirection, node.Key.position);
                        treeList.Add(start);
                    }
                }
            }
            //Part F - H
            List<Vector3> removeMeLater = new List<Vector3>();
            foreach(TreeNode point in treeList)
            {
                foreach (Vector3 attr in attractionList)
                {
                    if (Vector3.Distance(point.position, attr) <= killDistance)
                    {
                        removeMeLater.Add(attr);
                    }
                }
            }
            foreach (Vector3 remove in removeMeLater)
            {
                attractionList.Remove(remove);
            }
        }
    }

    private TreeNode instanceTreeBranch(TreeNode node, Vector3 growDirection, Vector3 pos)
    {
        float length = (killDistance / 2f);
        pos += growDirection.normalized * length;
        TreeNode tmp = new TreeNode(pos);
        node.next.Add(tmp);
        return tmp;
    }

    Vector3[] generateSpace(int numberOfPoints)
    {
        Vector3[] localSpace = new Vector3[numberOfPoints];

        Vector3 direction = Vector3.zero;
        for (int i = 0;i < numberOfPoints;i++)
        {
            direction = generateShape();
            localSpace[i] = gameObject.transform.position + direction + Vector3.up * spaceOffset;
        }
        return localSpace;
    }

    private Vector3 generateShape()
    {
        Vector3 direction = new Vector3();
        direction.x = Random.Range(-shapeScale.x, shapeScale.x);
        direction.y = shapeScale.y * Mathf.Sqrt(1 - (Mathf.Pow(direction.x, 2) / Mathf.Pow(shapeScale.x, 2)));
        direction.y = Random.Range(-direction.y, direction.y);
        direction.z = shapeScale.z * Mathf.Sqrt(1 - (Mathf.Pow(direction.x, 2) / Mathf.Pow(shapeScale.x, 2)) + (Mathf.Pow(direction.y, 2) / Mathf.Pow(shapeScale.y, 2)));
        direction.z = Random.Range(-direction.z, direction.z);

        return direction;
    }
}
