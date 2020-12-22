using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Tingyu Shen 260798146
// This is my class for creating rocks crates and mices
public class EnvironmentController : MonoBehaviour
{

    public GameObject treasurePrefab;
    public GameObject treasure;
    public GameObject monsterPrefab;
    public GameObject rockPrefab;
    public GameObject cratePrefab;

    public Player player;

    public List<GameObject> obstacles;

    bool iniMonster;

    public GameObject mice;

    // Start is called before the first frame update
    void Start()
    {
        obstacles = new List<GameObject>();
        iniMonster = false;
        player = FindObjectOfType<Player>();
        treasure = Instantiate(treasurePrefab);
        treasure.GetComponent<Renderer>().material.color = Color.red;
        treasure.transform.position = new Vector3(-30, 1, 0);
        treasure.name = "Treasure";
        addObstacleLine();
        addRocks();
        addCrates();

        generateMice();
    }

    // Update is called once per frame
    void Update()
    {
        createMonster();
    }

    void createMonster()
    {
        if (player.transform.position.z > -25 && !iniMonster)
        {
            GameObject monster = Instantiate(monsterPrefab);
            monster.transform.position = new Vector3(-25, 3, 0);
            iniMonster = true;
        }
    }

    // My method to generate a half-random obstacle line
    void addObstacleLine()
    {
        float iniX = -23f;
        float iniZ = Random.Range(-11f,-15f);
        for (int i = 0; i < 7; i++)
        {
            int whichOne = Random.Range(0, 2);
            GameObject obstacle;
            if (whichOne == 0)
            {
                obstacle = Instantiate(rockPrefab);
                obstacle.gameObject.name = "Rock";
                obstacle.transform.position = new Vector3(iniX, 2f, iniZ);
                obstacle.GetComponent<Renderer>().material.color = Color.black;
            }
            else
            {
                obstacle = Instantiate(cratePrefab);
                obstacle.gameObject.name = "Crate";
                obstacle.transform.position = new Vector3(iniX, 2f, iniZ);
                obstacle.GetComponent<Renderer>().material.color = Color.yellow;
            }

            obstacles.Add(obstacle);
            iniX += 3.05f;
            iniZ += Random.Range(0,3f);
        }
    }

    // My helper method to check rock/crate overlap
    public bool checkOverlap(Vector3 vector3)
    {
        for (int i = 0; i < obstacles.Count; i++)
        {
            if(obstacles[i] == null)
            {
                obstacles.RemoveAt(i);
            }
        }
        for (int i = 0; i<obstacles.Count; i++)
        {
            if(Mathf.Abs(obstacles[i].transform.position.x - vector3.x) < rockPrefab.transform.lossyScale.x && Mathf.Abs(obstacles[i].transform.position.z - vector3.z) < rockPrefab.transform.lossyScale.z)
            {
                return true;
            }
        }
        return false;
    }

    // My method for adding 10 random rocks
    void addRocks()
    {
        for(int i = 0; i < 10; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-23f, 23f), 2f, Random.Range(-23f, 23f));
            if (checkOverlap(pos))
            {
                i--;
            }
            else
            {
                GameObject rock = Instantiate(rockPrefab);
                rock.gameObject.name = "Rock";
                obstacles.Add(rock);
                rock.transform.position = pos;
                rock.GetComponent<Renderer>().material.color = Color.black;
            }
            
        }
    }

    // My method for adding 10 random crates
    void addCrates()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-23f, 23f), 2f, Random.Range(-23f, 23f));
            if (checkOverlap(pos))
            {
                i--;
            }
            else
            {
                GameObject crate = Instantiate(cratePrefab);
                crate.gameObject.name = "Crate";
                obstacles.Add(crate);
                crate.transform.position = pos;
                crate.GetComponent<Renderer>().material.color = Color.yellow;
            }

        }
    }

    // My method for generating 5 mices
    void generateMice()
    {
        for(int i = 0; i< 5; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-23.5f, 23.5f), 1f, Random.Range(-23.5f, 23.5f));
            if (checkOverlap(pos))
            {
                i--;
            }
            else
            {
                GameObject mice1 = Instantiate(mice);
                mice1.name = "mice";
                mice1.transform.position = pos;
            }
        }
    }

}
