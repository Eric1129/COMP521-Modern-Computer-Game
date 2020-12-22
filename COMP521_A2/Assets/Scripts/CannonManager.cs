using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Tingyu Shen 260798146
// This class is for manipulate two cannons
public class CannonManager : MonoBehaviour
{

    public GameObject Cannon1;
    public GameObject Cannon2;
    public GameObject Cannonball;

    // angle and velocity for 2 cannons
    public float angle1;
    public float angle2;
    public int muzzle1;
    public int muzzle2;

    // used for unity text on canvas to show stats
    public Text muzzle1Text;                                  
    public Text muzzle2Text;

    // check flag for current cannon
    public bool isLeft;

    // Initialize some default stats
    void Start()
    {
        isLeft = true;
        angle1 = 15f;
        angle2 = 15f;
        muzzle1 = 10;
        muzzle2 = 10;
        muzzle1Text.text = "Left Cannon Velocity: " + muzzle1;
        muzzle2Text.text = "Right Cannon Velocity: " + muzzle2;
    }

    // Update is called once per frame
    void Update()
    {
        // tab can change current cannon
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isLeft = !isLeft;
        }
        
        Direction();
        Fire();
        Muzzle();
    }

    // check if user press up arrow key or down arrow key
    // Then check current cannon
    // and change direction accordingly
    private void Direction()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))                                                     
        {
            if (isLeft)
            {
                if (angle1 < 90f)
                {
                    angle1 += 5f;
                    Cannon1.transform.Rotate(0, 0, 5f);
                }
            }
            else
            {
                if (angle2 < 90f)
                {
                    angle2 += 5f;
                    Cannon2.transform.Rotate(0, 0, -5f);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (isLeft)
            {
                if (angle1 > 0f)
                {
                    angle1 -= 5f;
                    Cannon1.transform.Rotate(0, 0, -5f);
                }
            }
            else
            {
                if (angle2 > 0f)
                {
                    angle2 -= 5f;
                    Cannon2.transform.Rotate(0, 0, 5f);
                }
            }
        }
    }

    // Check user presses space key
    // then check left or right
    // instantiate the cannonball accordingly
    private void Fire()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isLeft)
            {
                GameObject ball = Instantiate(Cannonball, Cannon1.transform.position + new Vector3(0,2,0), Quaternion.identity); // instantiate projectile
            }
            else
            {
                GameObject ball = Instantiate(Cannonball, Cannon2.transform.position + new Vector3(0,2,0), Quaternion.identity); // instantiate projectile
            }
        }

    }

    // Check user presses left arrow or right arrow key
    // then check left or right
    // change the muzzle value accordingly
    // I set the range 10~30
    private void Muzzle()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (isLeft)
            {
                if(muzzle1 > 10)
                {
                    muzzle1 -= 4;
                    muzzle1Text.text = "Left Cannon Velocity: " + muzzle1;
                }
               
            }
            else
            {
                if(muzzle2 > 10)
                {
                    muzzle2 -= 4;
                    muzzle2Text.text = "Right Cannon Velocity: " + muzzle2;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (isLeft)
            {
                if (muzzle1 < 30)
                {
                    muzzle1 += 4;
                    muzzle1Text.text = "Left Cannon Velocity: " + muzzle1;
                }
            }
            else
            {
                if (muzzle2 < 30)
                {
                    muzzle2 += 4;
                    muzzle2Text.text = "Right Cannon Velocity: " + muzzle2;
                }
            }
        }
    }
}
