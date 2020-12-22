using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tingyu Shen 260798146
// My class for spawn agent as well as keep log my experiment data
public class AgentController : MonoBehaviour
{
    // My unity prefabs
    public GameObject agent;

    // My editor variable
    public int agentNumber;
    public float ExecutionTime;

    public List<GameObject> agents;
    GameObject[] destinations;

    // Other class I need
    ReducedVisibilityGraph reducedVisibilityGraph;
    public bool agentStart;

    float currentTime;

    // Log data for final output
    public float TotalnumberOfPathsPlaned;
    public float TotalnumberOfReplanning;
    public float TotalnumberOfSuccess;
    public float TotalplanningTime;

    bool flag = true;
    // Start is called before the first frame update
    void Start()
    {
        agents = new List<GameObject>();
        destinations = new GameObject[agentNumber];
        reducedVisibilityGraph = GameObject.FindObjectOfType<ReducedVisibilityGraph>();
        agentStart = false;
        Invoke(nameof(spawnAgent), 0.1f);
    }


    // For experiment only
    private void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime >= ExecutionTime && flag)
        {
            int total = agents.Count;
            for(int i = 0; i<agents.Count; i++)
            {
                TotalnumberOfPathsPlaned += agents[i].GetComponent<Agent>().numberOfPathsPlaned;
                TotalnumberOfReplanning += agents[i].GetComponent<Agent>().numberOfReplanning;
                TotalnumberOfSuccess += agents[i].GetComponent<Agent>().numberOfSuccess;
                TotalplanningTime += agents[i].GetComponent<Agent>().planningTime;
                Destroy(agents[i]);
            }
            Debug.Log(TotalnumberOfPathsPlaned / total);
            Debug.Log(TotalnumberOfReplanning / total);
            Debug.Log(TotalnumberOfSuccess / total);
            Debug.Log(TotalplanningTime / total);
            flag = false;
        }
    }

    // My main spawn method for generating agent randomly in level with no overlapping
    void spawnAgent()
    {
        for (int i = 0; i < agentNumber; i++)
        {
            Vector3 initialPos = getRandomPosition();
            if (Physics.CheckSphere(initialPos, agent.transform.lossyScale.x/2f) || agentOverlap(initialPos))
            {
                i--;
            }
            else
            {
                GameObject agent1 = Instantiate(agent);
                var sphereRenderer = agent1.GetComponent<Renderer>();
                sphereRenderer.material.SetColor("_Color", new Color(Random.Range((float)0, 1), Random.Range((float)0, 1), Random.Range((float)0, 1)));
                agent1.transform.position = initialPos;
                agents.Add(agent1);
            }
        }

    }

    // This is my helper method for checking agent will overlap with other agents
    public bool agentOverlap(Vector3 a)
    {
        for(int i =0; i< agents.Count; i++)
        {
            if (Mathf.Abs(a.x - agents[i].transform.position.x) < agent.transform.lossyScale.x)
            {
                if (Mathf.Abs(a.z - agents[i].transform.position.z) < agent.transform.lossyScale.z)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // This is my method for generating random agent position in map
    public Vector3 getRandomPosition()
    {
        int Position = Random.Range(0, 25);

        if (Position == 0)
        {
            return new Vector3(Random.Range(-6.631f, -5.367f), 1.5f, Random.Range(-1.833f, -4.89f));
        }
        else if (Position == 1)
        {
            return new Vector3(Random.Range(-0.933f, 1f), 1.5f, Random.Range(0.09f, -4.89f));
        }
        else if (Position == 2)
        {
            return new Vector3(Random.Range(4.487f, 7.36f), 1.5f, Random.Range(-2.015f, -4.89f));
        }
        else if (Position == 3)
        {
            return new Vector3(Random.Range(10.207f, 12.933f), 1.5f, Random.Range(-7.353f, -8.726f));
        }
        else if (Position == 4)
        {
            return new Vector3(Random.Range(10.207f, 13.76f), 1.5f, Random.Range(-11.351f, -13.16f));
        }
        else if (Position == 5)
        {
            return new Vector3(Random.Range(5.3f, 7.208f), 1.5f, Random.Range(-15.215f, -17.58f));
        }
        else if (Position == 6)
        {
            return new Vector3(Random.Range(-1.93f, -0.19f), 1.5f, Random.Range(-15.215f, -16.54f));
        }
        else if (Position == 7)
        {
            return new Vector3(Random.Range(-6.75f, -4.98f), 1.5f, Random.Range(-15.215f, -19.68f));
        }
        else if (Position == 8)
        {
            return new Vector3(Random.Range(-12.497f, -10.1f), 1.5f, Random.Range(-11.365f, -12.636f));
        }
        else if (Position == 9)
        {
            return new Vector3(Random.Range(-13.91f, -10.1f), 1.5f, Random.Range(-7.33f, -8.647f));
        }
        else {
            return new Vector3(Random.Range((float)-9.57, (float)9.57), 1.5f, Random.Range((float)-5.3, (float)-14.7));
        }
    }
}
