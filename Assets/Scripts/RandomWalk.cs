using System.Collections.Generic;
using UnityEngine;

public class RandomWalk : MonoBehaviour
{
    public GameObject Stamm;
    private List<GameObject> treeObjects = new List<GameObject>();
    private List<GameObject> spacePoints = new List<GameObject>();

    public float twigLength = 0.5f;
    public float biasWheigt = 2.3f;
    public int gobackChance = 8; // chacnce 1 zu gobackChance 
    public int minTreeHieght = 4;
    public int randSeed = 323123;
    public int itterations = 40;

    private GrowHelper growHelper;

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
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            foreach (GameObject go in spacePoints)
            {
                go.SetActive(!go.activeSelf);
            }
        }
    }

    private void makeTree()
    {
        Random.InitState(randSeed);
        growHelper = GameObject.Find("GH").GetComponent<GrowHelper>();

        TreeNode start = new TreeNode(gameObject.transform.position);
        growTree(start, itterations);
        treeObjects = growHelper.generateTreeObjects(start, gameObject.transform, 10000);
        spacePoints = growHelper.renderTreePoints(start);
    }

    private void growTree(TreeNode start, int itterations)
    {
        Vector3 direction = gameObject.transform.up;
        Vector3 pos = Vector3.zero;
        pos = gameObject.transform.position;

        Stack<TreeNode> stack = new Stack<TreeNode>();
        TreeNode node = start;
        stack.Push(node);

        for (int i = 0;i < itterations;i++)
        {
            float bias = Random.Range(0, biasWheigt * (1/(1 + Vector3.Distance(pos, gameObject.transform.position))));
            direction = Random.onUnitSphere + gameObject.transform.up * bias;
            pos += direction.normalized * twigLength;

            node = new TreeNode(pos);
            stack.Peek().next.Add(node);
            stack.Push(node);

            if (stack.Count > minTreeHieght && Random.Range(0, (minTreeHieght * gobackChance)/(stack.Count)) == 0)
            {
                int goBack = Random.Range(1, stack.Count - minTreeHieght);
                for (int j = 0; j < goBack; j++)
                {
                    stack.Pop();
                }
                node = stack.Peek();
                pos = node.position;
            }
        }
    }

    public static Vector3 RandomDirectionAdditive(Transform t, float upWeight)
    {
        Vector3 rand = Random.onUnitSphere;
        Vector3 biased = rand + t.up * upWeight;
        return biased.normalized;
    }
}
