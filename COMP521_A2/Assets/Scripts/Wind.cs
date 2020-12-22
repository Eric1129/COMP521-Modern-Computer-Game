using UnityEngine;
using UnityEngine.UI;

// Tingyu Shen 260798146
// This class is for generating wind
public class Wind : MonoBehaviour
{
    public Generator generator;

    // public variables for other class use
    public float altitude;
    public float wind_force = 0f;
    float force_range = 0.0000065f;
    float timer = 2f;

    // show current situation on unity text
    public Text windText;

    void Start()
    {
        generator = FindObjectOfType<Generator>();
        altitude = generator.mountainTop; // set wind altitude as highest point of mountain
        ChangeWindForce();
    }

    // Update is called once per frame
    void Update()
    {
        //Call ChangeWindForce() every 2s
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            ChangeWindForce();
            timer = 2f;
        }

    }

    // This method is for changing wind force
    // by using preset randge with my random 
    void ChangeWindForce()
    {
        //random a wind_force in (-force_range, force_range)
        wind_force = force_range * (new System.Random(System.Guid.NewGuid().GetHashCode()).Next(0, 200) - 100) / 1000f;
        string direction;
        if (wind_force < 0)
        {
            direction = "left";
        }
        else
        {
            direction = "right";
        }
        windText.text = "Current wind direction is " + direction + " with speed of " + Mathf.Abs(wind_force) * 10000000;
    }
}
