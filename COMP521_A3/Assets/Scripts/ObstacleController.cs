using UnityEngine;
using System.Collections.Generic;

// Tingyu Shen 260798146
// This is class for obstacle genrator
public class ObstacleController : MonoBehaviour
{
    // My unity prefab
    public GameObject obstacle;
    int obstacleNumber;

    public GameObject point;

    List<Obstacle> obstacles = new List<Obstacle>();

    // store my reflex points for RVG
    public Vector3[] LShapePoints;

    void Start()
    {
        BuildObstacle();
    }

    // Method for building obstacles
    void BuildObstacle()
    {
        // get 3 or 4 obstacles
        obstacleNumber = Random.Range(3, 5);
        LShapePoints = new Vector3[obstacleNumber*5];
        for (int i = 0; i < obstacleNumber; i++)
        {
            float xScale1 = Random.Range(2f, 4f);
            float zScale1 = Random.Range(1f, 1.5f);

            Vector3 obstacle1_Scale = new Vector3(xScale1, 1, zScale1);
            Vector3 obstacle1_Position = new Vector3(Random.Range((float)-9.57 + xScale1 / 2f,
                (float)9.57 - xScale1 / 2f), 1.5f, Random.Range((float)-5.3 - zScale1 / 2f, (float)-14.7 + zScale1 / 2f));

            Obstacle myObstacle = new Obstacle(obstacle1_Position, obstacle1_Scale);

            int LShape = Random.Range(1, 5);
            float xScale2 = Random.Range(1f, 1.5f);
            float zScale2 = Random.Range(2f, 4f);

            float offsetX = (xScale1 - xScale2) / 2f;
            float offsetZ = (zScale2 - zScale1) / 2f;

            Vector3 obstacle2_Scale = new Vector3(xScale2, 1, zScale2);
            Vector3 obstacle2_Position = new Vector3(0, 0, 0);

            // handle out of boundary case
            if (LShape == 1 || LShape == 2)
            {
                if ((obstacle1_Position.z + offsetZ + zScale2 / 2f) > -5.3f)
                {
                    LShape += 2;
                }
            }
            else
            {
                if ((obstacle1_Position.z - offsetZ - zScale2 / 2f) < -14.7f)
                {
                    LShape -= 2;
                }
            }
            // upleft
            if (LShape == 1)
            {
                obstacle2_Position = obstacle1_Position + new Vector3(-offsetX, 0, +offsetZ);
                LShapePoints[0 + i * 5] = obstacle2_Position + new Vector3(-obstacle2_Scale.x / 2f -0.001f, 0, obstacle2_Scale.z / 2f + 0.001f);
                LShapePoints[1 + i * 5] = obstacle2_Position + new Vector3(obstacle2_Scale.x / 2f + 0.001f, 0, obstacle2_Scale.z / 2f + 0.001f);
                LShapePoints[2 + i * 5] = obstacle1_Position + new Vector3(-obstacle1_Scale.x / 2f - 0.001f, 0, -obstacle1_Scale.z / 2f - 0.001f);
                LShapePoints[3 + i * 5] = obstacle1_Position + new Vector3(obstacle1_Scale.x / 2f + 0.001f, 0, -obstacle1_Scale.z / 2f - 0.001f);
                LShapePoints[4 + i * 5] = obstacle1_Position + new Vector3(obstacle1_Scale.x / 2f + 0.001f, 0, obstacle1_Scale.z / 2f + 0.001f);
            }
            //upright
            if (LShape == 2)
            {
                obstacle2_Position = obstacle1_Position + new Vector3(+offsetX, 0, +offsetZ);
                LShapePoints[0 + i * 5] = obstacle2_Position + new Vector3(-obstacle2_Scale.x / 2f - 0.001f, 0, obstacle2_Scale.z / 2f + 0.001f);
                LShapePoints[1 + i * 5] = obstacle2_Position + new Vector3(obstacle2_Scale.x / 2f + 0.001f, 0, obstacle2_Scale.z / 2f+ 0.001f);
                LShapePoints[2 + i * 5] = obstacle1_Position + new Vector3(obstacle1_Scale.x / 2f + 0.001f, 0, -obstacle1_Scale.z / 2f- 0.001f);
                LShapePoints[3 + i * 5] = obstacle1_Position + new Vector3(-obstacle1_Scale.x / 2f- 0.001f, 0, -obstacle1_Scale.z / 2f- 0.001f);
                LShapePoints[4 + i * 5] = obstacle1_Position + new Vector3(-obstacle1_Scale.x / 2f- 0.001f, 0, obstacle1_Scale.z / 2f+ 0.001f);
            }
            //downleft
            if (LShape == 3)
            {
                obstacle2_Position = obstacle1_Position + new Vector3(-offsetX, 0, -offsetZ);
                LShapePoints[0 + i * 5] = obstacle2_Position + new Vector3(-obstacle2_Scale.x / 2f- 0.001f, 0, -obstacle2_Scale.z / 2f- 0.001f);
                LShapePoints[1 + i * 5] = obstacle2_Position + new Vector3(obstacle2_Scale.x / 2f+ 0.001f, 0, -obstacle2_Scale.z / 2f- 0.001f);
                LShapePoints[2 + i * 5] = obstacle1_Position + new Vector3(-obstacle1_Scale.x / 2f- 0.001f, 0, obstacle1_Scale.z / 2f+ 0.001f);
                LShapePoints[3 + i * 5] = obstacle1_Position + new Vector3(obstacle1_Scale.x / 2f+ 0.001f, 0, obstacle1_Scale.z / 2f+ 0.001f);
                LShapePoints[4 + i * 5] = obstacle1_Position + new Vector3(obstacle1_Scale.x / 2f+ 0.001f, 0, -obstacle1_Scale.z / 2f- 0.001f);
            }
            // downright
            if (LShape == 4)
            {
                obstacle2_Position = obstacle1_Position + new Vector3(+offsetX, 0, -offsetZ);
                LShapePoints[0 + i * 5] = obstacle2_Position + new Vector3(-obstacle2_Scale.x / 2f- 0.001f, 0, -obstacle2_Scale.z / 2f- 0.001f);
                LShapePoints[1 + i * 5] = obstacle2_Position + new Vector3(obstacle2_Scale.x / 2f+ 0.001f, 0, -obstacle2_Scale.z / 2f- 0.001f);
                LShapePoints[2 + i * 5] = obstacle1_Position + new Vector3(obstacle1_Scale.x / 2f+ 0.001f, 0, obstacle1_Scale.z / 2f+ 0.001f);
                LShapePoints[3 + i * 5] = obstacle1_Position + new Vector3(-obstacle1_Scale.x / 2f- 0.001f, 0, obstacle1_Scale.z / 2f+ 0.001f);
                LShapePoints[4 + i * 5] = obstacle1_Position + new Vector3(-obstacle1_Scale.x / 2f- 0.001f, 0, -obstacle1_Scale.z / 2f- 0.001f);
            }

            Obstacle myObstacle2 = new Obstacle(obstacle2_Position, obstacle2_Scale);

            bool overlap = false;
            for (int j = 0; j < obstacles.Count; j++)
            {
                if (obstacleOverlap(obstacles[j], myObstacle))
                {
                    overlap = true;
                }
            }
            for (int j = 0; j < obstacles.Count; j++)
            {
                if (obstacleOverlap(obstacles[j], myObstacle2))
                {
                    overlap = true;
                }
            }

            if (overlap)
            {
                i--;
            }
            else
            {
                obstacles.Add(myObstacle);
                obstacles.Add(myObstacle2);
                
                GameObject obstacle1 = Instantiate(obstacle);
                obstacle1.transform.localScale = obstacle1_Scale;
                obstacle1.transform.position = obstacle1_Position;

                GameObject obstacle2 = Instantiate(obstacle);
                obstacle2.transform.localScale = obstacle2_Scale;
                obstacle2.transform.position = obstacle2_Position;

                Renderer renderer = obstacle1.GetComponent<Renderer>();
                renderer.material.SetColor("_Color", Color.red);
                Renderer renderer1 = obstacle2.GetComponent<Renderer>();
                renderer1.material.SetColor("_Color", Color.red);
            }
        }
    }

    // Helper method for checking two obstacles are overlap
    bool obstacleOverlap(Obstacle a, Obstacle b)
    {
        if (a.position.x + a.scale.x / 2f > b.position.x - b.scale.x / 2f)
        {
            if (a.position.x - a.scale.x / 2f < b.position.x + b.scale.x / 2f)
            {
                if (a.position.z + a.scale.z / 2f > b.position.z - b.scale.z / 2f)
                {
                    if (a.position.z - a.scale.z / 2f < b.position.z + b.scale.z / 2f)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
