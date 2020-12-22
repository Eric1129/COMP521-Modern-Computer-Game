using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tingyu Shen 260798146
// My class for crate behavior
public class Crate : MonoBehaviour
{
    Player player;
    // Start is called before the first frame update
    bool destroy = false;
    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // if crate collide with anything after monster throw it, it will crash
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if(transform.position.y > 2f)
            {
                if (!player.toggled && !destroy)
                {
                    player.lifeLeft--;
                }
                
                Destroy(gameObject);
                destroy = true;
            }
        }
        else if (collision.gameObject.name == "Rock")
        {
            Destroy(gameObject);
            destroy = true;
        }
       
    }
}
