using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public GameObject menu, startscreen, deathscreen, endscreen;
    public GameObject movement, movement2;

    private const string HubName = "HubName";
    public string sceneName;
    void OnGUI()
    {
        Scene scene = SceneManager.GetActiveScene();
        sceneName = scene.name;
    }
    public string GetString(string KeyName)
    {
        return PlayerPrefs.GetString(KeyName);
    }
    public void DeleteKey(string KeyName)
    {
        PlayerPrefs.DeleteKey(KeyName);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("m") && !startscreen.activeSelf && !deathscreen.activeSelf && !endscreen.activeSelf)
        {//Open menu panel
            if (menu.activeSelf)
            {//Close the menu if its already open
                menu.SetActive(false);
                Time.timeScale = 1;
                movement.GetComponent<Character2DController>().gamePaused = false;
                AphroditeRoomActive();
            }
            else if (!menu.activeSelf)
            {
                menu.SetActive(true);              
                movement.GetComponent<Character2DController>().gamePaused = true;
                Time.timeScale = 0;
                AphroditeRoomInactive();
            }
        }
    }
    public void ButtonResumeScene()
    {//Resume the scene
        menu.SetActive(false);
        Time.timeScale = 1;
        movement.GetComponent<Character2DController>().gamePaused = false;
        AphroditeRoomActive();
    }
    public void ButtonResetScene()
    {//Restart the current scene
        SceneManager.LoadScene(sceneName);
    }
    public void HubScene()
    {//Return the player to the hub
        if (!PlayerPrefs.HasKey("PlayerCoins"))
        {
            PlayerPrefs.SetInt("PlayerCoins", 0);
            PlayerPrefs.Save();
        }
        if(PlayerPrefs.HasKey(HubName))
        {//If the player is continuing from a previous save, load the last scene the player was in
            SceneManager.LoadScene(PlayerPrefs.GetString(HubName));
        }
        else
        {//Start the player in the first scene of the game
            SceneManager.LoadScene("Hub2");
        }
        

    }
    public void NewGameScene()
    {//New game removes all previous saved data
        if (sceneName == "StartScene")
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("PlayerCoins", 0);
            
            PlayerPrefs.Save();
        }
        SceneManager.LoadScene("Hub2");

    }
    public void ButtonExitScene()
    {//Return to the game start scene
        SceneManager.LoadScene("StartScene");
    }
    public void ButtonStartScene()
    {//Start the room the player is in
        startscreen.SetActive(false);
        Time.timeScale = 1;
    }
    public void ButtonExitGame()
    {//Close the game
        Application.Quit();
    }
    public void AphroditeRoomActive()
    {
        if(sceneName == "AphroditeRoom")
        {
            movement2.GetComponent<Character2DController>().gamePaused = false;
        }
    }
    public void AphroditeRoomInactive()
    {
        if (sceneName == "AphroditeRoom")
        {
            movement2.GetComponent<Character2DController>().gamePaused = true;
        }
    }
}
