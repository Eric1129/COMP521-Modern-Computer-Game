using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tingyu Shen 260798146
// Class for steering behavior for Mice
// get some ideas from https://gamedevelopment.tutsplus.com/tutorials/understanding-steering-behaviors-collision-avoidance--gamedev-7777
public class Mice : MonoBehaviour
{

    EnvironmentController environmentController;
    private Vector3 newVelocity, steering, velocity, target;
    private float maxVelocity;
    private float maxSteerForce = 0.5f;
    private float maxAvoidForce = 0.2f;
    private float wanderTime;

    private RaycastHit raycastHit;
    private Rigidbody rb;

    void Start()
    {
        environmentController = FindObjectOfType<EnvironmentController>();
        rb = gameObject.GetComponent<Rigidbody>();
        maxVelocity = 0.1f;
        newDestination();
    }

    void FixedUpdate()
    {
        // set a new des every 5 seconds
        if (wanderTime < 0)
        {
            wanderTime = 5f;
            newDestination();
        }
        // Pause behavior for every 5 seconds puse 0.5 seconds
        else if (wanderTime < 0.5f)
        {
            velocity = new Vector3(0, 0, 0);
        }
        else
        {
            Seek();
        }

        // If mice get to target, it seeks to new destination
        if (Mathf.Abs(transform.position.x - target.x) < 1f && Mathf.Abs(transform.position.z - target.z) < 1f)
        {
            newDestination();
        }

        wanderTime -= Time.deltaTime;
    }

    // My helper method for generating new destination
    private void newDestination()
    {
        target = new Vector3(Random.Range(-23.5f, 23.5f), 1f, Random.Range(-23.5f, 23.5f));
        while (environmentController.checkOverlap(target))
        {
            target = new Vector3(Random.Range(-23.5f, 23.5f), 1f, Random.Range(-23.5f, 23.5f));
        }
    }

    // My steering behavior for seeking
    private void Seek()
    {
        Vector3 avoidForce = getAvoidForce();

        if (avoidForce.magnitude > 0)
        {
            steering = avoidForce;
        }
        else
        {
            newVelocity = Vector3.Normalize(target - transform.position);
            newVelocity *= maxVelocity;

            steering = newVelocity - velocity;

            if (steering.magnitude > maxSteerForce)
            {
                steering.Normalize();
                steering *= maxSteerForce;
            }
        }

        velocity += steering;
        if (velocity.magnitude > maxVelocity)
        {
            velocity.Normalize();
            velocity *= maxVelocity;
        }

        rb.MovePosition(transform.position + velocity);
    }

    // This is my method for generating avoid force
    private Vector3 getAvoidForce()
    {
        Vector3 avoidance = new Vector3(0, 0, 0);
        int obstacleSituation = detectObstacle();

        if (obstacleSituation < 0)
        {
            return avoidance;
        }
        if (obstacleSituation == 1)
        {
            if (transform.position.z > 0)
            {
                avoidance.x = raycastHit.normal.z;
                avoidance.z = -raycastHit.normal.x;
            }
            else
            {
                avoidance.x = -raycastHit.normal.z;
                avoidance.z = raycastHit.normal.x;
            }
        }
        if (obstacleSituation == 2)
        {
            avoidance.x = -raycastHit.normal.z;
            avoidance.z = raycastHit.normal.x;
        }
        if (obstacleSituation == 3)
        {
            avoidance.x = raycastHit.normal.z;
            avoidance.z = -raycastHit.normal.x;
        }
        if (obstacleSituation == 4 && raycastHit.collider.gameObject.name == "mice")
        {
            avoidance = raycastHit.normal;
        }
        if (obstacleSituation == 5 && raycastHit.collider.gameObject.name == "mice")
        {
            avoidance = raycastHit.normal;
        }
        avoidance.Normalize();
        avoidance *= maxAvoidForce;
        return avoidance;
    }


    // This is my helper method to detect obstacles
    // including player monster rock and crate
    // Find the closest obstacle
    private int detectObstacle()
    {
        Vector3 offset1 = new Vector3(-velocity.z, 0, velocity.x);
        offset1.Normalize();
        offset1 *= 0.5f;

        Vector3 offset2 = new Vector3(velocity.z, 0, -velocity.x);
        offset2.Normalize();
        offset2 *= 0.5f;

        RaycastHit raycastHit1, raycastHit2, raycastHit3, raycastHit4, raycastHit5;

        float maxDetectScope = 3f;

        float d1 = -1, d2 = -1, d3 = -1, d4 = -1, d5 = -1;

        float closestDistance = float.PositiveInfinity;

        if (Physics.Raycast(transform.position, velocity, out raycastHit1, maxDetectScope))
        {
            d1 = Vector3.Distance(transform.position, raycastHit1.transform.position);
            if(d1 < closestDistance)
            {
                closestDistance = d1;
                
            }
        }
        if (Physics.Raycast(transform.position + offset1, velocity, out raycastHit2, maxDetectScope))
        {
            d2 = Vector3.Distance(transform.position, raycastHit2.transform.position);
            if(d2 < closestDistance)
            {
                closestDistance = d2;
                
            }
        }
        if (Physics.Raycast(transform.position + offset2, velocity, out raycastHit3, maxDetectScope))
        {
            d3 = Vector3.Distance(transform.position, raycastHit3.transform.position);

            if(d3 < closestDistance)
            {
                closestDistance = d3;
                
            }
        }
        if (d1 < 0 && d2 < 0 && d3 < 0 && Physics.Raycast(transform.position, offset1, out raycastHit4, maxDetectScope - 0.5f))
        {
            d4 = Vector3.Distance(transform.position, raycastHit4.transform.position);

            if(d4 < closestDistance)
            {
                closestDistance = d4;
            }
        }
        if (d1 < 0 && d2 < 0 && d3 < 0 && Physics.Raycast(transform.position, offset1, out raycastHit5, maxDetectScope - 0.5f))
        {
            d5 = Vector3.Distance(transform.position, raycastHit5.transform.position);
            if(d5 < closestDistance)
            {
                closestDistance = d5;
            }
        }

        if (d1 == closestDistance)
        {
            raycastHit = raycastHit1;
            return 1;
        }
        else if (d2 == closestDistance)
        {
            raycastHit = raycastHit2;
            return 2;
        }
        else if (d3 == closestDistance)
        {
            raycastHit = raycastHit3;
            return 3;
        }
        else if (d4 == closestDistance)
        {
            Physics.Raycast(transform.position, offset1, out raycastHit, maxDetectScope - 0.5f);
            return 4;
        }
        else if (d5 == closestDistance)
        {
            Physics.Raycast(transform.position, offset2, out raycastHit, maxDetectScope - 0.5f);
            return 5;
        }
        return -1;
    }

    // Mice behavior for hitted by rock or crate
    private void OnCollisionEnter(Collision collision)
    {
        if((collision.gameObject.name == "Rock" || collision.gameObject.name == "Crate") && collision.gameObject.transform.position.y > 2f)
        {
            Destroy(gameObject);
        }
    }
}
