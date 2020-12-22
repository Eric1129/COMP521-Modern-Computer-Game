using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle
{

    public Vector3 position;
    public Vector3 scale;


    public Obstacle(Vector3 position, Vector3 scale)
    {
        this.position = position;
        this.scale = scale;
    }
}
