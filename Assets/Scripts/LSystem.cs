using System.Collections.Generic;
using UnityEngine;

public class LSystem : MonoBehaviour
{
    /*
     * lengthScale l = z.b 0.8
     * angle a = Random.Vector3 in cone
     * angle b = Random.Vector3 in cone
     * branchAngle c = small Random in cone
     * split []
     * 
     * F(5)A
     * A -> F(1)[cF(1)A]a[cF(1)A]b[cF(1)A]
     * F(x) -> F(x*l)
     * 
     */
    public GameObject Stamm;
    private List<GameObject> treeObjects = new List<GameObject>();
    private List<GameObject> spacePoints = new List<GameObject>();

    public Vector2 lenght = new Vector2(1.01f, 1.11f);
    public Vector2 a = new Vector2(40, 150);
    public Vector2 b = new Vector2(40, 150);
    public Vector2 c = new Vector2(10, 60);
    public float twigLenght = 0.5f;
    public int randSeed = 3123123;
    public string initalSeed = "F(1.5)A";
    public int itterations = 5;

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
        if (randSeed != 0)
        {
            Random.InitState(randSeed);
        }
        else
        {
            Random.InitState((int)(Time.deltaTime * 1000));
        }
            growHelper = GameObject.Find("GH").GetComponent<GrowHelper>();

        TreeNode start = new TreeNode(gameObject.transform.position);
        createTree(start, initalSeed, itterations);
        treeObjects = growHelper.generateTreeObjects(start, gameObject.transform, 10000);
        spacePoints = growHelper.renderTreePoints(start);
    }

    public void createTree(TreeNode start, string seed, int height)
    {
        string word = createString(seed, height);
        Debug.Log(word);

        Quaternion rotation = Quaternion.identity;
        Vector3 pos = Vector3.zero;
        pos = gameObject.transform.position;
        rotation = gameObject.transform.rotation;
        Stack<KeyValuePair<TreeNode, Quaternion>> stack = new Stack<KeyValuePair<TreeNode, Quaternion>>();
        TreeNode node = start;
        stack.Push(KeyValuePair.Create(node, rotation));

        for (int i = 0; i < word.Length;i++)
        {
            switch (word[i])
            {
                case 'F':
                    float number = 0;
                    string s = word.Substring(i + 2, word.IndexOf(')', i) - (i + 2));
                    float.TryParse(s, out number);
                    if (number == 0)
                    {
                        Debug.Log("error number = 0, L system F");
                    }
                    else
                    {
                        i += 2 + s.Length;
                        pos += rotation * Vector3.up * number;
                        TreeNode temp = new TreeNode(pos);
                        node.next.Add(temp);
                        node = temp;
                    }
                        break;
                case 'a':
                    rotation *= Quaternion.AngleAxis(Random.Range(a.x, a.y), Vector3.up);
                    break;
                case 'b':
                    rotation *= Quaternion.AngleAxis(Random.Range(b.x, b.y), Vector3.up);
                    break;
                case 'c':
                    rotation *= Quaternion.AngleAxis(Random.Range(c.x, c.y), Vector3.forward);
                    break;
                case '[':
                    stack.Push(KeyValuePair.Create(node, rotation));
                    break;
                case ']':
                    KeyValuePair<TreeNode, Quaternion> kvp = stack.Pop();
                    node = kvp.Key;
                    pos = node.position;
                    rotation = kvp.Value;
                    break;
            }
        }
    }

    public string createString(string start, int iteration)
    {
        string tempString = "";
        string add = "";
        for (int i = 0; i < start.Length;i++)
        {
            switch (start[i])
            {
                case 'F':
                    float n = -1;
                    string s = start.Substring(i + 2, start.IndexOf(')', i) - (i + 2));
                    float.TryParse(s, out n);
                    if (n == -1 || n == 0)
                    {
                        Debug.Log("n = " + n + " iteration = " + iteration);
                    }
                    add = "F(" + (n * Random.Range(lenght.x, lenght.y)).ToString() + ")";
                    tempString = tempString + add;
                    i = start.IndexOf(")", i);
                    break;
                case 'A':
                    float a = twigLenght;
                    add = "F("+a+")[cF("+a+")A]a[cF("+a+")A]b[cF("+a+")A]";
                    tempString = tempString + add;
                    break;
                default:
                    tempString += start[i];
                    break;
            }
        }
        if (iteration > 0)
        {
            tempString = createString(tempString, --iteration);
        }
        return tempString;
    }
}
