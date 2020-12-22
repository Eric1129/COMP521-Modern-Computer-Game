using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Tingyu Shen 260798146
// This is my class for player
public class Player : MonoBehaviour
{
    public Text shieldText;
    int shieldValue;

    public Text ShieldOn;
    public Text TreasureText;
    EnvironmentController environmentController;

    public bool toggled;
    float timeLag;

    bool getTreasure;

    public Text gameResult;

    public Text CurrentStep;
    public Text CurrentPlan;

    public Text life;
    public int lifeLeft;

    void Start()
    {
        lifeLeft = 2;
        getTreasure = false;
        environmentController = FindObjectOfType<EnvironmentController>();
        toggled = false;
        shieldValue = 10;
        shieldText.text = "Shield Value: " + shieldValue;
    }

    // Track shield on or off
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            toggled = !toggled;
            timeLag = 0;
        }

        if (toggled && shieldValue > 0)
        {
            ShieldOn.text = "Shield On!!!";
            timeLag += Time.deltaTime;
            if ( timeLag - Time.deltaTime> 1)
            {
                shieldValue--;
                shieldText.text = "Shield Value: " + shieldValue;
                timeLag = Time.deltaTime;
            }
        
        }
        else
        {
            ShieldOn.text = "Shield Off";
        }

        checkWin();
        checkLose();
    }

    // Get treasure
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Treasure")
        {
            getTreasure = true;
            Destroy(environmentController.treasure);
            TreasureText.text = "Get the treasure!";
        }
    }

    void checkWin()
    {
        if(transform.position.z < -25 && getTreasure)
        {
            gameResult.text = "Game Win!";
            Invoke("endSimulation", 5f);
        }
    }

    void checkLose()
    {
        life.text = "Player's life left: " + lifeLeft;
        if (lifeLeft == 0)
        {
            gameResult.text = "Game Lose!";
            Invoke("endSimulation", 5f);
        }
    }

    void endSimulation()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
