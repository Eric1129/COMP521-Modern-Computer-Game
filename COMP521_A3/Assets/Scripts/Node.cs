using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tingyu Shen 260798146
// My node class to store current node sphere and its neighbours
// and its f/g value
public class Node
{
    public Vector3 currentNodePosition;
    public List<Node> neibourNodes;
    public float fOfN;
    public float gOfN;

    public Node()
    {
        fOfN = 99999;
        gOfN = 99999;
        neibourNodes = new List<Node>();
    }
}
