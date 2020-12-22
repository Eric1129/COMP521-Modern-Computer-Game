using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tingyu Shen 260798146
// This class is used for genrate entire reduced visibility graph
public class ReducedVisibilityGraph : MonoBehaviour
{
    Vector3[] levelPoints;
    public GameObject point;

    GameObject[] LevelSpheres;

    public bool graphFinished = false;

    ObstacleController obstacleController;

    public GameObject[] EntireReducedVisibilitySpheres;

    // Since start method in unity is not serialized
    // I manually ask it waits for obstacles to build finish
    // then draw reduced visibily graphs
    void Start()
    {
        obstacleController = GameObject.FindObjectOfType<ObstacleController>();
        Invoke(nameof(drawReducedVisibilityGraph), 0.05f);
    }

    public void drawReducedVisibilityGraph()
    {
        obstacleController = FindObjectOfType<ObstacleController>();
        addLevelNodes();
        EntireReducedVisibilitySpheres = new GameObject[20 + obstacleController.LShapePoints.Length];
        for(int i = 0; i < LevelSpheres.Length; i++)
        {
            EntireReducedVisibilitySpheres[i] = LevelSpheres[i];
        }

        for (int i = 0; i < obstacleController.LShapePoints.Length; i++)
        {
            GameObject tempPoint = Instantiate(point);
            tempPoint.transform.position = obstacleController.LShapePoints[i];
            tempPoint.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            EntireReducedVisibilitySpheres[20 + i] = tempPoint;
        }

        for (int i = 0; i < EntireReducedVisibilitySpheres.Length; i++)
        {
            for(int j = i+1; j < EntireReducedVisibilitySpheres.Length; j++)
            {
                
                if(drawLine(EntireReducedVisibilitySpheres[i].transform.position, EntireReducedVisibilitySpheres[j].transform.position))
                {
                    var go = new GameObject();
                    var lr = go.AddComponent<LineRenderer>();
                    lr.SetPosition(1, EntireReducedVisibilitySpheres[i].transform.position);
                    lr.SetPosition(0, EntireReducedVisibilitySpheres[j].transform.position);
                    lr.startWidth = 0.03f;
                    lr.endWidth = 0.03f;
                }
            }
        }  
    }

    // level geometries
    public void addLevelNodes()
    {
        levelPoints = new Vector3[20];
        levelPoints[0] = new Vector3(-7.21f, 1.5f, -5.05f);
        levelPoints[1] = new Vector3(-4.796f, 1.5f, -5.05f);
        levelPoints[2] = new Vector3(-1.23f, 1.5f, -5.05f);
        levelPoints[3] = new Vector3(1.24f, 1.5f, -5.05f);
        levelPoints[4] = new Vector3(4.25f, 1.5f, -5.05f);
        levelPoints[5] = new Vector3(7.65f, 1.5f, -5.05f);
        levelPoints[6] = new Vector3(9.95f, 1.5f, -7.04f);
        levelPoints[7] = new Vector3(9.95f, 1.5f, -9.01f);
        levelPoints[8] = new Vector3(9.95f, 1.5f, -11.04f);
        levelPoints[9] = new Vector3(9.95f, 1.5f, -13.34f);
        levelPoints[10] = new Vector3(7.459f, 1.5f, -14.94f);
        levelPoints[11] = new Vector3(5.072f, 1.5f, -14.94f);
        levelPoints[12] = new Vector3(0.12f, 1.5f, -14.94f);
        levelPoints[13] = new Vector3(-2.257f, 1.5f, -14.94f);
        levelPoints[14] = new Vector3(-4.64f, 1.5f, -14.94f);
        levelPoints[15] = new Vector3(-7.036f, 1.5f, -14.94f);
        levelPoints[16] = new Vector3(-9.95f, 1.5f, -12.934f);
        levelPoints[17] = new Vector3(-9.95f, 1.5f, -11.05f);
        levelPoints[18] = new Vector3(-9.95f, 1.5f, -8.94f);
        levelPoints[19] = new Vector3(-9.95f, 1.5f, -7.05f);

        LevelSpheres = new GameObject[20];

        for (int i = 0; i < levelPoints.Length; i++)
        {
            GameObject tempPoint = Instantiate(point);
            tempPoint.transform.position = levelPoints[i];
            tempPoint.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            LevelSpheres[i] = tempPoint;
        }
    }

    // My draw line methods
    public bool drawLine(Vector3 startObject, Vector3 endObject)
    {
        Vector3 newStartPos = extendLine(startObject, endObject, ExtendDirection.START_POINT, 0.15f);
        Vector3 newEndPos = extendLine(startObject, endObject, ExtendDirection.END_POINT, 0.15f);

        if(!Physics.Linecast(newStartPos, newEndPos) && !Physics.Linecast(newEndPos, newStartPos))
        {
            return true;
        }
        return false;
    }

    Vector3 extendLine(Vector3 startPoint, Vector3 endPoint, ExtendDirection extendDirection, float extendDistance)
    {
        Ray ray = new Ray();

        //Start
        if (extendDirection == ExtendDirection.START_POINT)
        {
            ray.origin = startPoint;
            ray.direction = startPoint - endPoint;
        }

        //End
        else if (extendDirection == ExtendDirection.END_POINT)
        {
            ray.origin = endPoint;
            ray.direction = endPoint - startPoint;
        }

        //Extend
        Vector3 newUnityPoint = ray.GetPoint(extendDistance);
        //Debug.DrawLine(ray.origin, newUnityPoint, Color.blue);
        return newUnityPoint;
    }

    public enum ExtendDirection
    {
        START_POINT, END_POINT
    }

}
