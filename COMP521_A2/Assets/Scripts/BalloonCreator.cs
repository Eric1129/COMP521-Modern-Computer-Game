using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tingyu Shen 260798146
// This class is to generate balloon
public class BalloonCreator : MonoBehaviour
{
    // public variable to load the prefab
    public Balloon balloon;

    float timer = 1f;
    public Vector3 initialPoint = new Vector3();

    // call generator to get water position
    Generator generator;

    void Start()
    {
        generator = FindObjectOfType<Generator>();
        CreateNewBallon();
    }

    void Update()
    {
        // produce one balloon every 1 sec
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            CreateNewBallon();
            timer = 1f;
        }
    }
    void CreateNewBallon()
    {
        // the sprawn point is in the watery area
        initialPoint = new Vector3(Random.Range(-8, 8), Random.Range(generator.waterlevel-3, generator.waterlevel-5), 0);
        transform.position = initialPoint;
        Instantiate(balloon, transform.position, Quaternion.identity);
    }

}
