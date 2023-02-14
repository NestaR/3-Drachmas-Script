using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchHub : MonoBehaviour
{
    public string sceneToLoad, sceneName;
    public GameObject player;
    void Start()
    {
        Scene scene = SceneManager.GetActiveScene();
        sceneName = scene.name;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {//Lets the player go between each of the available hubs (level select) by checking which side of the map the player is on and in which room
        if (collision.CompareTag("Player"))
        {
            if (sceneName == "Hub" && player.transform.position.x > 0)
            {
                sceneToLoad = "EndScene";
            }
            else if (sceneName == "Hub")
            {
                sceneToLoad = "Hub4";
            }
            else if (sceneName == "Hub2" && player.transform.position.x > 0)
            {
                sceneToLoad = "Hub3";
            }
            else if (sceneName == "Hub3" && player.transform.position.x < 0)
            {
                sceneToLoad = "Hub2";
            }
            else if (sceneName == "Hub3" && player.transform.position.x > 0)
            {
                sceneToLoad = "Hub4";
            }
            else if (sceneName == "Hub4" && player.transform.position.x < 0)
            {
                sceneToLoad = "Hub3";
            }
            else if (sceneName == "Hub4" && player.transform.position.x > 0)
            {
                sceneToLoad = "Hub";
            }
            StartCoroutine(LoadYourAsyncScene());
        }
    }
    IEnumerator LoadYourAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
