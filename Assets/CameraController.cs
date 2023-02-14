using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CameraController : MonoBehaviour
{
    public GameObject player, player2;
    public Vector3 offset, playerPos;
    public float leftClamp, rightClamp;
    public GameObject startscreen;
    public bool playerFollowed;
    public string sceneName;
    // Start is called before the first frame update
    void Start()
    {
        Scene scene = SceneManager.GetActiveScene();
        sceneName = scene.name;
    }

    // Update is called once per frame
    void Update()
    {

        if(!startscreen.activeSelf)
        {//Follow the player when the start button is pressed
            if(transform.position.x > leftClamp && !playerFollowed)
            {
                playerPos = new Vector3(player.transform.position.x + offset.x, player.transform.position.y + offset.y, player.transform.position.z + offset.z);
                transform.position = Vector3.Lerp(transform.position, playerPos, Time.deltaTime * 0.3f);
            }
            else
            {
                playerFollowed = true;
                player.GetComponent<Character2DController>().enabled = true;

                if(sceneName == "AphroditeRoom")
                {
                    player2.GetComponent<Character2DController>().enabled = true;
                }
                transform.position = new Vector3(player.transform.position.x + offset.x, player.transform.position.y + offset.y, player.transform.position.z + offset.z);

                if (player.transform.position.x + offset.x < leftClamp)
                {//Prevent the camera from going past a certain point
                    transform.position = new Vector3(leftClamp, player.transform.position.y + offset.y, player.transform.position.z + offset.z);
                }
                else if (player.transform.position.x + offset.x > rightClamp)
                {
                    transform.position = new Vector3(rightClamp, player.transform.position.y + offset.y, player.transform.position.z + offset.z);
                }
            }
        }
    }
}
