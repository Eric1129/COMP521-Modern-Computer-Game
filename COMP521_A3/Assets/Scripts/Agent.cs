using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tingyu Shen 260798146
// This is my class for agent simulation
public class Agent : MonoBehaviour
{
    Node start;
    Node Destination;
    public GameObject desCube;

    List<Node> currentGraph;
    List<Node> fringe;

    List<GameObject> lines;

    GameObject destination;

    ReducedVisibilityGraph reducedVisibilityGraph;
    AgentController agentController;

    GameObject go;
    LineRenderer lr;

    List<Node> totalPath;

    GameObject lastContered;

    bool initialized = false;

    public int numberOfPathsPlaned;
    public int numberOfReplanning;
    public int numberOfSuccess;
    public float planningTime;

    int FailedTemps;

    Node lastNode;

    float waitTime;
    float waitTimeTotal;

    // Initialization needs to be after reduced visibility graph
    private void Start()
    {
        numberOfPathsPlaned = 0;
        numberOfReplanning = 0;
        numberOfSuccess = 0;
        agentController = FindObjectOfType<AgentController>();
        reducedVisibilityGraph = FindObjectOfType<ReducedVisibilityGraph>();
        Invoke(nameof(StartANewRound), 0.1f);
    }

    // this is the method for starting a completely new round with new destination
    void StartANewRound()
    {
        FailedTemps = 0;
        destination = Instantiate(desCube);
        var desRenderer = destination.GetComponent<Renderer>();
        var lll = GetComponent<Renderer>();
        desRenderer.material.SetColor("_Color", lll.material.GetColor("_Color"));
        destination.transform.position = findDestination();
        startNewAttemp();
    }

    // Each attemp for path finding
    void startNewAttemp()
    {
        float p = Time.realtimeSinceStartup;
        waitTimeTotal = 0;
        waitTime = Random.Range(0.1f, 0.5f);
        numberOfPathsPlaned++;
        start = new Node();
        currentGraph = new List<Node>();
        Destination = new Node();
        lastNode = new Node();
        start.currentNodePosition = transform.position;
        Destination.currentNodePosition = destination.transform.position;

        for (int i = 0; i < reducedVisibilityGraph.EntireReducedVisibilitySpheres.Length; i++)
        {
            Node temp = new Node();
            temp.currentNodePosition = reducedVisibilityGraph.EntireReducedVisibilitySpheres[i].transform.position;
            currentGraph.Add(temp);
        }
        createNewGraph(start, Destination);
        AStarPathFinding();
        planningTime += Time.deltaTime;
    }

    // My update method for continually path finding
    private void Update()
    {

        if (initialized)
        {
            if (agentOverlapForOtherAgents(transform.position))
            {
                waitTimeTotal += Time.deltaTime;
                if (waitTimeTotal > waitTime)
                {
                    initialized = false;
                    backOff(lastNode.currentNodePosition);
                    FailedTemps++;
                    if(FailedTemps < 3)
                    {
                        for (int i = 0; i < lines.Count; i++)
                        {
                            Destroy(lines[i]);
                        }
                        numberOfReplanning++;
                        startNewAttemp();
                    }
                    else
                    {
                        for (int i = 0; i < lines.Count; i++)
                        {
                            Destroy(lines[i]);
                        }
                        Destroy(destination);
                        StartANewRound();
                        return;
                    }
                }
            }
            else if (moveToPoint(totalPath[totalPath.Count - 1].currentNodePosition))
            {
                if (totalPath.Count > 1)
                {
                    lastNode.currentNodePosition = totalPath[totalPath.Count - 1].currentNodePosition;
                    totalPath.RemoveAt(totalPath.Count - 1);
                }
                else
                {
                    initialized = false;
                    Destroy(destination);
                    numberOfSuccess++;
                    for (int i = 0; i < lines.Count; i++)
                    {
                        Destroy(lines[i]);
                    }
                    float waitTime = Random.Range(0.2f, 1);
                    Invoke(nameof(StartANewRound), waitTime);
                    return;
                }
            }
        }
    }

    // This is my helper method for checking agent will overlap with other agents (except itself)
    public bool agentOverlapForOtherAgents(Vector3 a)
    {
        for (int i = 0; i < agentController.agents.Count; i++)
        {
            if(agentController.agents[i].transform != transform){
                if (Mathf.Abs(a.x - agentController.agents[i].transform.position.x) < transform.lossyScale.x)
                {
                    if (Mathf.Abs(a.z - agentController.agents[i].transform.position.z) < transform.lossyScale.z)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    // My helper method for agent to find a destination
    Vector3 findDestination()
    {
        Vector3 des = agentController.getRandomPosition();

        while (Physics.CheckSphere(des, Mathf.Sqrt(Mathf.Pow(desCube.transform.lossyScale.x, 2)) / 2f))
        {
            des = agentController.getRandomPosition();
        }
        return des;
    }

    // My helper method for moving agent
    bool moveToPoint(Vector3 c)
    {
        float step = 10 * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, c, step);
        if (transform.position.x == c.x && transform.position.z == c.z)
        {
            return true;
        }
        else
        {
            go = new GameObject();
            lr = go.AddComponent<LineRenderer>();
            lr.SetPosition(1, c);
            lr.SetPosition(0, transform.position);
            lr.startWidth = 0.03f;
            lr.endWidth = 0.03f;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.material.SetColor("_Color", transform.GetComponent<Renderer>().material.GetColor("_Color"));
            lines.Add(go);
            return false;
        }
    }

    // My helper method for moving agent
    bool backOff(Vector3 c)
    {
        float step = 30 * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, c, step);
        if (transform.position.x == c.x && transform.position.z == c.z)
        {
            return true;
        }
        else
        {
            go = new GameObject();
            lr = go.AddComponent<LineRenderer>();
            lr.SetPosition(1, c);
            lr.SetPosition(0, transform.position);
            lr.startWidth = 0.03f;
            lr.endWidth = 0.03f;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.material.SetColor("_Color", transform.GetComponent<Renderer>().material.GetColor("_Color"));
            lines.Add(go);
            return false;
        }
    }

    // My helper method to generate a new graph for this agent at current search
    void createNewGraph(Node startNode, Node endNode)
    {
        lines = new List<GameObject>();
        currentGraph.Add(startNode);
        currentGraph.Add(endNode);
        for (int i = 0; i < currentGraph.Count; i++)
        {
            for (int j = i + 1; j < currentGraph.Count; j++)
            {
                if (reducedVisibilityGraph.drawLine(currentGraph[i].currentNodePosition, currentGraph[j].currentNodePosition))
                {
                    currentGraph[i].neibourNodes.Add(currentGraph[j]);
                    currentGraph[j].neibourNodes.Add(currentGraph[i]);
                }
            }
        }
    }

    // My A* main method
    // Basically follow what Clark taught in class
    void AStarPathFinding()
    {
        fringe = new List<Node>();
        fringe.Add(start);
        start.gOfN = 0;
        start.fOfN = Vector3.Distance(start.currentNodePosition, Destination.currentNodePosition);
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();

        while (fringe.Count != 0)
        {
            Node c = extractSmallestF(fringe);
            if (c == Destination)
            {
                reconstruct(cameFrom, c);
                break;
            }
            for (int i = 0; i < c.neibourNodes.Count; i++)
            {
                float distance = c.gOfN + Vector3.Distance(c.currentNodePosition, c.neibourNodes[i].currentNodePosition);
                if (distance < c.neibourNodes[i].gOfN)
                {
                    cameFrom[c.neibourNodes[i]] = c;
                    c.neibourNodes[i].gOfN = distance;
                    c.neibourNodes[i].fOfN = c.neibourNodes[i].gOfN + Vector3.Distance(c.neibourNodes[i].currentNodePosition, Destination.currentNodePosition);
                    if (!fringe.Contains(c.neibourNodes[i]))
                    {
                        fringe.Add(c.neibourNodes[i]);
                    }
                }
            }
        }
    }

    // My helper method for A* to extract the node with smallest f value in fringe
    Node extractSmallestF(List<Node> fringe)
    {
        Node minNode = null;
        float minF = 999;
        for (int i = 0; i < fringe.Count; i++)
        {
            if (fringe[i].fOfN < minF)
            {
                minF = fringe[i].fOfN;
                minNode = fringe[i];
            }
        }
        fringe.Remove(minNode);
        return minNode;
    }

    // My helper method to reconstruct the shortest path
    List<Node> reconstruct(Dictionary<Node,Node> cameFrom, Node current)
    {
        totalPath = new List<Node>();
        totalPath.Add(current);
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Add(current);
        }
        initialized = true;
        return totalPath;
    }
}
