using System.Collections.Generic;
using UnityEngine;

public class TreeNode
{

    public Vector3 position;
    public List<TreeNode> next = new List<TreeNode>();

    public TreeNode(Vector3 position)
    {
        this.position = position;
    }

    //returns the remaining twig length
    public int LongestTwig()
    {
        int count = 0;
        foreach (TreeNode node in next)
        {
            int length = node.LongestTwig();
            if (length > count)
            {
                count = length;
            }
        }
        count++;
        return count;
    }
}
