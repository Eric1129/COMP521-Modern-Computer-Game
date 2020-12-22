using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tingyu Shen 260798146
// My class for Rock behavior
public class Rock : MonoBehaviour
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

    // if Rock collide with player only after monster throw it, it will crash
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player" && transform.position.y > 2f )
        {
            if (!player.toggled && !destroy)
            {
                player.lifeLeft--;
            }
            
            Destroy(gameObject);
            destroy = true;
        }
    }
}
