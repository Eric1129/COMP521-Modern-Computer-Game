using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tingyu Shen 260798146
// This class if for cannonball
public class Cannonball : MonoBehaviour
{

    CannonManager Manager;
    Generator generator;
    Balloon[] balloon_list;

    float initialv, vx, vy, ax, ay;
    float radius;
    bool isLeft;
    float angle;

    // Use this for initialization
    void Start()
    {
        ax = 0;
        //Find GameObject
        generator = GameObject.FindObjectOfType<Generator>();
        Manager = GameObject.FindObjectOfType<CannonManager>();
        isLeft = Manager.isLeft;

        radius = gameObject.GetComponent<SpriteRenderer>().bounds.size.x / 2;

        //Initiate cannonball velocity and accelerations
        if (isLeft)
        {
            initialv = Manager.muzzle1/100f;
            angle = Manager.angle1;
            vx = initialv * Mathf.Cos(angle * Mathf.Deg2Rad);
            vy = initialv * Mathf.Sin(angle * Mathf.Deg2Rad);
        }
        else
        {
            initialv = Manager.muzzle2/100f;
            angle = Manager.angle2;
            vx = -initialv * Mathf.Cos(angle * Mathf.Deg2Rad);
            vy = initialv * Mathf.Sin(angle * Mathf.Deg2Rad);
        }
        ay = -0.00098f;
    }

    // Update is called once per frame
    void Update()
    {

        //Compute current velocity and position of cannonball
        vx = vx + ax * 1f;
        vy = vy + ay * 1f;

        CollisionWithBalloon();

        // If exceed screen bounds
        //If get into water
        //If stop moving, make the cannonball disappear
        if (ExceedScreenBoundsLeftRight() || GetIntoWater())
        {
            Destroy(gameObject);
        }
        if (StopMoving())
        {
            Destroy(gameObject,1);
        }
        int col = CollideWithTerrain();
        if (col!=0)
        {
            CollisionResolutionWithMountain(col);
        }

        gameObject.transform.position = new Vector3(transform.position.x + vx * 1f, transform.position.y + vy * 1f, 0);
    }

    int CollideWithTerrain()
    {
        //Search for each line segment if the cannonball intersects with it
        for (int i = 0; i < generator.index; i++)
        {
            //If collide, make the cannonball bounce
            //Angles alpha refers to the intersection angle between the line segment and horizontal line
            //Beta refers to the intersection angle between the current velocity and horizontal line
            if (Mathf.Pow(generator.TerrainPoints[i].x - transform.position.x, 2) + Mathf.Pow(generator.TerrainPoints[i].y - transform.position.y, 2) <= Mathf.Pow(radius, 2))
                return i;
        }
        return 0;
    }


    void CollisionResolutionWithMountain(int i)
    {
        if (i <= 48 || i >= 244)
        {
            CollisionResolutionWithGround(i);
        }
        else
        {
            float alpha = Mathf.Atan2(generator.TerrainPoints[i+1].y - generator.TerrainPoints[i - 1].y,
        generator.TerrainPoints[i+1].x - generator.TerrainPoints[i - 1].x == 0 ? 0.0001f : generator.TerrainPoints[i+1].x - generator.TerrainPoints[i - 1].x);
            float v0 = Mathf.Sqrt(vx * vx + vy * vy);
            float v1 = v0 * (new System.Random(System.Guid.NewGuid().GetHashCode()).Next(0, 46) + 50) / 75f;

            float beta = Mathf.Atan2(vy, vx == 0 ? 0.0001f : vx);
            vx = v1 * Mathf.Cos(alpha * 2 + beta);
            vy = v1 * Mathf.Sin(alpha * 2 + beta);

            if (Mathf.Abs(vy) < Mathf.Abs(ay))
            {
                vy = 0;
                ay = 0;
            }
            if (Mathf.Abs(vx) < Mathf.Abs(ay))
            {
                vx = 0;
            }
        }
        
    }

    // since for ground we just need to bounce as reflection
    void CollisionResolutionWithGround(int i)
    {
        if(Mathf.Abs(vy) < Mathf.Abs(ay) && i!=0)
        {
            vy = 0;
            ay = 0;
        }

        if(Mathf.Abs(vx) < Mathf.Abs(ay) && i!=0)
        {
            vx = 0;
        }

        float rand = (new System.Random(System.Guid.NewGuid().GetHashCode()).Next(0, 46) + 50) / 100f;
        vy = -vy * rand;
        vx = vx * rand;
 
    }

    // Get out of screen except up
    bool ExceedScreenBoundsLeftRight()
    {
        if (transform.position.x > 36f || transform.position.x < -36f)
            return true;
        return false;
    }

    // if the cannonball hit into water
    bool GetIntoWater()
    {
        for (int i = 0; i < generator.WaterPoints.Count; i++)
        {
            if (Mathf.Pow(generator.WaterPoints[i].x - transform.position.x, 2) + Mathf.Pow(generator.WaterPoints[i].y - transform.position.y, 2) <= Mathf.Pow(radius, 2))
            {
                return true; 
            }
        }
        return false;
            
    }

    //return true if velocity from all directions are 0
    bool StopMoving()
    {
        if (vx == 0 && vy == 0)
            return true;
        return false;
    }


    // This is for collide to balloon
    // It loops to monitor all balloons in game
    // and all it's balloon part of points
    // if intersect
    // call balloon's destroy method
    void CollisionWithBalloon()
    {
        balloon_list = FindObjectsOfType<Balloon>();
        for (int i = 0; i < balloon_list.Length; i++)
        {
            Balloon ballon = balloon_list[i].GetComponent<Balloon>();
            for (int j = 0; j < ballon.points.Count - 4; j++)
            {
                float distance = Mathf.Sqrt(Mathf.Pow(ballon.points[j].x+ ballon.initiate_point.x - transform.position.x, 2) +
                    Mathf.Pow(ballon.points[j].y+ ballon.initiate_point.y - transform.position.y, 2));
                if (distance <= radius)
                {
                    ballon.DestroyB();
                }
            }
        }
    }

}
