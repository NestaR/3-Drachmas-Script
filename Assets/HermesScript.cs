using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HermesScript : MonoBehaviour
{
    public GameObject player;
    public Text WorldTimer;
    public float worldTimer, timer;

    void Update()
    {     
        if(player.GetComponent<RoomScript>().numLetters < 3)
        {//If the player hasn't delivered all the letters the timer will run
            timer += Time.deltaTime;
        }     
        if (timer >= 1)
        {
            timer = 0;
            worldTimer -= 1;
            WorldTimer.text = worldTimer.ToString();
        }
        else if(worldTimer <= 0)
        {//When the timer runs out the level will end
            player.GetComponent<Player>().lives = 0;
            timer = 0;
        }
    }
}
