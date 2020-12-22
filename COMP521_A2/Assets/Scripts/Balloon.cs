using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tingyu Shen 260798146
// This is class for balloon behavior
public class Balloon : MonoBehaviour
{

    LineRenderer lineRenderer;

    // to get wind altittue
    Wind wind;

    // This is
    // since balloon upwards with constant speed, no need for ay
    private float ax, vx, vy;
    public Vector3 initiate_point = new Vector3(0,0,0);

    // To store balloon shape
    public List<Vector3> points = new List<Vector3>();

    // The entire ballon original position
    private List<Vector3> original = new List<Vector3>();

    // This is flag for if collided by cannonball
    public bool destry;

    public List<Vector3> Oldpoints;

    public bool first = true;
    // Initially give a speed upward and no horizontal speed
    void Start()
    {   
        destry = false;
        initiate_point = gameObject.transform.position;
        lineRenderer = GetComponent<LineRenderer>();
        wind = GameObject.FindObjectOfType<Wind>();

        CreateBalloon();
        vy = 0.05f;
        vx = 0;
    }

    // Since wind speed as 
    void Update()
    {

        BalloonMoveWithConstraints();
        MaintainTailConstraints();

        if (ExceedScreenBounds(points[points.Count - 1]))
        {
            DestroyB();
        }

        if (destry)
        {
            Destroy(gameObject);
        }

        DrawBalloon();
    }


    // Create balloon point lists
    public void CreateBalloon()
    {
        points.Add(new Vector3(0, 1, 0));
        points.Add(new Vector3(-1, 2, 0));
        points.Add(new Vector3(-1, 3, 0));
        points.Add(new Vector3(0, 4, 0));
        points.Add(new Vector3(1, 3, 0));
        points.Add(new Vector3(1, 2, 0));
        points.Add(new Vector3(0, 1, 0));
        points.Add(new Vector3(0, 0, 0));
        points.Add(new Vector3(0, -1, 0));
        points.Add(new Vector3(0, -2, 0));
        points.Add(new Vector3(0, -3, 0));

        // add them into previous location
        for (int i = 0; i < points.Count; i++)
        {
            original.Add(new Vector3(points[i].x, points[i].y, 0));
        }

    }

    void DrawBalloon()
    {
        //Set LineRenderer params and draw all points in a for loop
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.useWorldSpace = false;
        lineRenderer.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++)
        {
            lineRenderer.SetPosition(i, points[i]);
        }

    }

    // This is used for maintain balloon constraint
    // Uses verlet strategy
    // Use old pos and current pos to create new pos
    void BalloonMoveWithConstraints()
    {
        for (int i = 0; i < points.Count - 4; i++)
        {
            Vector3 newPoint = new Vector3();
            if (points[i].y + initiate_point.y > wind.altitude)
            {
                ax += wind.wind_force;
            }

            // For first time move
            // it needs add vertical change to make previous point
            if (first)
            {
                newPoint = 2 * points[i] - original[i] + new Vector3(ax, vy, 0);
            }
            // Then for second time we only need to use previous pos and current pos to
            // produce next pos
            else
            {
                newPoint = 2 * points[i] - original[i] + new Vector3(ax, 0, 0);
            }
            original[i] = points[i];
            points[i] = newPoint;
            // check out of bound situation

        }
        first = false;

    }

    void BallonConstraint()
    {

    }

    // This is used for maintain tail constrains
    // it only checks tail points which is index 7 and after
    // It uses the strategy I mentioned in pdf to compute the distance between ajacent points
    // and use similar triangle to decide where to be 
    void MaintainTailConstraints()
    {
        for (int i = 7; i < points.Count; i++)
        {
            float xD = (points[i - 1].x - points[i].x);
            float yD = (points[i - 1].y - points[i].y);
            float disctance = Mathf.Sqrt(Mathf.Pow(xD, 2) + Mathf.Pow(yD, 2));
            if (disctance > 1f)
            {
                points[i] = new Vector3(points[i].x + (disctance - 1) * xD / disctance, points[i].y+ (disctance - 1) * yD / disctance, 0);
            }
        }
    }

    // This is used for balloon exceeds screen scenario 
    bool ExceedScreenBounds(Vector3 vector3)
    {
        if (vector3.x + initiate_point.x > 36 || vector3.y + initiate_point.y < -36 || vector3.y + initiate_point.y > 20f)
            return true;
        return false;
    }

    public void DestroyB()
    {
        Destroy(gameObject);
    }
}
