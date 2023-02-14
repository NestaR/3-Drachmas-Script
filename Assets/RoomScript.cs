using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomScript : MonoBehaviour
{
    public GameObject ZeusLetterIndicator, AresLetterIndicator, AthenaLetterIndicator;
    [Header("Rooms")]
    public GameObject ZeusRoom, PoseidonRoom, HadesRoom;
    public GameObject ZeusRoomCoin, PoseidonRoomCoin, HadesRoomCoin;

    public GameObject HeraRoom, DemeterRoom, AresRoom;
    public GameObject HeraRoomCoin, DemeterRoomCoin, AresRoomCoin;

    public GameObject ApolloRoom, ArtemisRoom, AthenaRoom;
    public GameObject ApolloRoomCoin, ArtemisRoomCoin, AthenaRoomCoin;

    public GameObject HephaestusRoom, AphroditeRoom, HermesRoom;
    public GameObject HephaestusRoomCoin, AphroditeRoomCoin, HermesRoomCoin;

    public bool poseidonRoom, zeusRoom, hadesRoom;
    public bool heraRoom, demeterRoom, aresRoom;
    public bool apolloRoom, artemisRoom, athenaRoom;
    public bool hephaestusRoom, aphroditeRoom, hermesRoom;
    public GameObject NextHubArrow, PreviousHubArrow, PayWall, PayText, BluePlat, RedPlat, GreenPlat, GreenBarrier, ZeusLetterIcon, AresLetterIcon, AthenaLetterIcon;
    [Header("Other")]
    [Space(1)]
    private Animator chestanimator;
    public bool onChest, onPay, onBlue, onRed, onGreen, zeusLetter, aresLetter, athenaLetter, submitLetter, submitWrongLetter;
    public string sceneToLoad, sceneName;
    public int numLetters;
    BoxCollider2D chestCollider;
    public GameObject coin;
    public GameObject chestCover, player;
    Vector3 pos;
    AudioSource source;
    public AudioClip doorClose;
    private const string PlayerHubPosition = "PlayerHubPosition", PlayerCoins = "PlayerCoins", HubName = "HubName";
    private const string ZeusCoin = "ZeusCoin", PoseidonCoin = "PoseidonCoin", HadesCoin = "HadesCoin";
    private const string HeraCoin = "HeraCoin", DemeterCoin = "DemeterCoin", AresCoin = "AresCoin";
    private const string ApolloCoin = "ApolloCoin", ArtemisCoin = "ArtemisCoin", AthenaCoin = "AthenaCoin";
    private const string HephaestusCoin = "HephaestusCoin", AphroditeCoin = "AphroditeCoin", HermesCoin = "HermesCoin";
    // Start is called before the first frame update
    void Start()
    {
        Scene scene = SceneManager.GetActiveScene();
        sceneName = scene.name;
        chestanimator = GetComponent<Animator>();
        chestCollider = GetComponent<BoxCollider2D>();
        source = GetComponent<AudioSource>();
        if (PlayerPrefs.GetInt("PayWall2") >= 1 && sceneName == "Hub2")
        {//Check if the player has already accessed a paywall
            PayWall.SetActive(false);
            PayText.SetActive(false);
        }
        else if (PlayerPrefs.GetInt("PayWall3") >= 1 && sceneName == "Hub3")
        {
            PayWall.SetActive(false);
            PayText.SetActive(false);
        }
        else if (PlayerPrefs.GetInt("PayWall4") >= 1 && sceneName == "Hub4")
        {
            PayWall.SetActive(false);
            PayText.SetActive(false);
        }
        else if (PlayerPrefs.GetInt("PayWall") >= 1 && sceneName == "Hub")
        {
            PayWall.SetActive(false);
            PayText.SetActive(false);
        }
    }
    public void SetFloat(string KeyName, float Value)
    {
        PlayerPrefs.SetFloat(KeyName, Value);
    }
    public void SetString(string KeyName, string Value)
    {
        PlayerPrefs.SetString(KeyName, Value);
    }
    public void SetInt(string KeyName, int Value)
    {
        PlayerPrefs.SetInt(KeyName, Value);
    }
    public int GetInt(string KeyName)
    {
        return PlayerPrefs.GetInt(KeyName);
    }
    // Update is called once per frame
    void Update()
    {
        if(numLetters >= 3)
        {
            player.GetComponent<Player>().stageActivate.SetActive(true);
        }
        if (Input.GetKeyDown("e") || Input.GetKeyDown("w") || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if ((sceneName == "Hub" || sceneName == "Hub2" || sceneName != "Hub3" || sceneName != "Hub4") && sceneToLoad != "")
            {//Checks which hub the player is in to enter a selected level
                //Stores the location for when the player exits the room
                source.PlayOneShot(doorClose, 1f);
                PlayerPrefs.SetFloat(PlayerHubPosition, transform.position.x);
                PlayerPrefs.SetString(HubName, sceneName);
                PlayerPrefs.Save();
                StartCoroutine(LoadYourAsyncScene());
            }
            else if (onChest)
            {//If the player is next to a chest it will open when interacting
                chestCover.SetActive(false);
                coin.SetActive(true);
                Invoke("activateCoin", 2);
                chestanimator.SetTrigger("Opened");
                onChest = false;
                chestCollider.enabled = false;               
            }
            else if(onPay)
            {//Take 3 coins to access the next level
                int currentCoins = PlayerPrefs.GetInt(PlayerCoins);

                if(PlayerPrefs.GetInt(PlayerCoins) >= 3)
                {
                    currentCoins -= 3;
                    this.gameObject.GetComponent<Player>().setCoins(currentCoins);
                    this.gameObject.GetComponent<Player>().numCoins = currentCoins;
                    PlayerPrefs.SetInt(PlayerCoins, currentCoins);
                    if(sceneName == "Hub2")
                    {
                        PlayerPrefs.SetInt("PayWall2", 1);
                    }
                    else if (sceneName == "Hub3")
                    {
                        PlayerPrefs.SetInt("PayWall3", 1);
                    }
                    else if (sceneName == "Hub4")
                    {
                        PlayerPrefs.SetInt("PayWall4", 1);
                    }
                    else if (sceneName == "Hub")
                    {
                        PlayerPrefs.SetInt("PayWall", 1);
                    }
                    this.GetComponent<Player>().coinSound();
                    PayWall.SetActive(false);
                    PayText.SetActive(false);
                    PlayerPrefs.Save();
                }
                else
                {
                    this.GetComponent<Player>().deniedSound();
                    Debug.Log("Insufficient funds");
                }
            }
            else if(onBlue)
            {//Activate blue objects
                BluePlat.SetActive(true);
                RedPlat.SetActive(false);
            }
            else if (onRed)
            {//Activate red objects
                RedPlat.SetActive(true);
            }
            else if (onGreen)
            {//Activate green objects
                GreenPlat.SetActive(true);
                GreenBarrier.SetActive(false);
                BluePlat.SetActive(false);
            }
            else if(submitLetter)
            {//Submit a letter collected to a room
                if(zeusLetter)
                {
                    ZeusLetterIndicator.SetActive(true);
                    ZeusLetterIcon.SetActive(false);
                    zeusLetter = false;
                    numLetters += 1;
                }
                else if (aresLetter)
                {
                    AresLetterIndicator.SetActive(true);
                    AresLetterIcon.SetActive(false);
                    aresLetter = false;
                    numLetters += 1;
                }
                else if (athenaLetter)
                {
                    AthenaLetterIndicator.SetActive(true);
                    AthenaLetterIcon.SetActive(false);
                    athenaLetter = false;
                    numLetters += 1;
                }
                this.GetComponent<Player>().coinSound();
            }
            else if(submitWrongLetter)
            {
                this.GetComponent<Player>().deniedSound();
                Debug.Log("Wrong Letter!!!");
            }
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ZeusRoom")
        {
            //When the player is near a room they are able to enter
            if (sceneName == "HermesRoom")
            {
                submitWrongLetter = true;
            }
            else if (sceneName == "Hub")
            {
                ZeusRoom.SetActive(true);
                zeusRoom = true;
                sceneToLoad = "ZeusRoom";
                if (PlayerPrefs.HasKey(ZeusCoin))
                {//Shows the player if a coin for that room has been collected
                    ZeusRoomCoin.SetActive(true);
                }
                else
                {
                    ZeusRoomCoin.SetActive(false);
                }
            }
        }
        else if (collision.gameObject.tag == "PoseidonRoom")
        {
            if (sceneName == "HermesRoom" && athenaLetter)
            {
                submitLetter = true;
            }
            else if(sceneName == "HermesRoom" && !athenaLetter)
            {
                submitWrongLetter = true;
            }
            else if (sceneName == "Hub")
            {
                PoseidonRoom.SetActive(true);
                poseidonRoom = true;
                sceneToLoad = "PoseidonRoom";
                if (PlayerPrefs.HasKey(PoseidonCoin))
                {
                    PoseidonRoomCoin.SetActive(true);
                }
                else
                {
                    PoseidonRoomCoin.SetActive(false);
                }
            }
        }
        else if (collision.gameObject.tag == "HadesRoom")
        {
            if (sceneName == "HermesRoom")
            {
                submitWrongLetter = true;
            }
            else if (sceneName == "Hub")
            {
                HadesRoom.SetActive(true);
                hadesRoom = true;
                sceneToLoad = "HadesRoom";
                if (PlayerPrefs.HasKey(HadesCoin))
                {
                    HadesRoomCoin.SetActive(true);
                }
                else
                {
                    HadesRoomCoin.SetActive(false);
                }
            }
        }
        else if (collision.gameObject.tag == "HeraRoom")
        {
            if(sceneName == "HermesRoom" && zeusLetter)
            {
                submitLetter = true;
            }
            else if (sceneName == "HermesRoom" && !zeusLetter)
            {
                submitWrongLetter = true;
            }
            else if (sceneName == "Hub2")
            {
                HeraRoom.SetActive(true);
                heraRoom = true;
                sceneToLoad = "HeraRoom";
                if (PlayerPrefs.HasKey(HeraCoin))
                {
                    HeraRoomCoin.SetActive(true);
                }
                else
                {
                    HeraRoomCoin.SetActive(false);
                }
            }

        }
        else if (collision.gameObject.tag == "DemeterRoom")
        {
            if (sceneName == "HermesRoom")
            {
                submitWrongLetter = true;
            }
            else if (sceneName == "Hub2")
            {
                DemeterRoom.SetActive(true);
                demeterRoom = true;
                sceneToLoad = "DemeterRoom";
                if (PlayerPrefs.HasKey(DemeterCoin))
                {
                    DemeterRoomCoin.SetActive(true);
                }
                else
                {
                    DemeterRoomCoin.SetActive(false);
                }
            }
        }
        else if (collision.gameObject.tag == "AresRoom")
        {
            if (sceneName == "HermesRoom")
            {
                submitWrongLetter = true;
            }
            else if (sceneName == "Hub2")
            {
                AresRoom.SetActive(true);
                aresRoom = true;
                sceneToLoad = "AresRoom";
                if (PlayerPrefs.HasKey(AresCoin))
                {
                    AresRoomCoin.SetActive(true);
                }
                else
                {
                    AresRoomCoin.SetActive(false);
                }
            }

        }
        else if (collision.gameObject.tag == "ApolloRoom")
        {
            if (sceneName == "HermesRoom")
            {
                submitWrongLetter = true;
            }
            else if (sceneName == "Hub3")
            {
                ApolloRoom.SetActive(true);
                apolloRoom = true;
                sceneToLoad = "ApolloRoom";
                if (PlayerPrefs.HasKey(ApolloCoin))
                {
                    ApolloRoomCoin.SetActive(true);
                }
                else
                {
                    ApolloRoomCoin.SetActive(false);
                }
            }

        }
        else if (collision.gameObject.tag == "ArtemisRoom")
        {
            if (sceneName == "HermesRoom")
            {
                submitWrongLetter = true;
            }
            else if (sceneName == "Hub3")
            {
                ArtemisRoom.SetActive(true);
                artemisRoom = true;
                sceneToLoad = "ArtemisRoom";
                if (PlayerPrefs.HasKey(ArtemisCoin))
                {
                    ArtemisRoomCoin.SetActive(true);
                }
                else
                {
                    ArtemisRoomCoin.SetActive(false);
                }
            }
        }
        else if (collision.gameObject.tag == "AthenaRoom")
        {
            if (sceneName == "HermesRoom")
            {
                submitWrongLetter = true;
            }
            else if (sceneName == "Hub3")
            {
                AthenaRoom.SetActive(true);
                athenaRoom = true;
                sceneToLoad = "AthenaRoom";
                if (PlayerPrefs.HasKey(AthenaCoin))
                {
                    AthenaRoomCoin.SetActive(true);
                }
                else
                {
                    AthenaRoomCoin.SetActive(false);
                }
            }
        }
        else if (collision.gameObject.tag == "HephaestusRoom")
        {
            if (sceneName == "HermesRoom")
            {
                submitWrongLetter = true;
            }
            else if (sceneName == "Hub4")
            {
                HephaestusRoom.SetActive(true);
                hephaestusRoom = true;
                sceneToLoad = "HephaestusRoom";
                if (PlayerPrefs.HasKey(HephaestusCoin))
                {
                    HephaestusRoomCoin.SetActive(true);
                }
                else
                {
                    HephaestusRoomCoin.SetActive(false);
                }
            }
        }
        else if (collision.gameObject.tag == "AphroditeRoom")
        {
            if (sceneName == "HermesRoom" && aresLetter)
            {
                submitLetter = true;
            }
            else if (sceneName == "HermesRoom" && !aresLetter)
            {
                submitWrongLetter = true;
            }
            else if (sceneName == "Hub4")
            {
                AphroditeRoom.SetActive(true);
                aphroditeRoom = true;
                sceneToLoad = "AphroditeRoom";
                if (PlayerPrefs.HasKey(AphroditeCoin))
                {
                    AphroditeRoomCoin.SetActive(true);
                }
                else
                {
                    AphroditeRoomCoin.SetActive(false);
                }
            }
        }
        else if (collision.gameObject.tag == "HermesRoom")
        {
            if (sceneName == "HermesRoom")
            {
                submitWrongLetter = true;
            }
            else if (sceneName == "Hub4")
            {
                HermesRoom.SetActive(true);
                hermesRoom = true;
                sceneToLoad = "HermesRoom";
                if (PlayerPrefs.HasKey(HermesCoin))
                {
                    HermesRoomCoin.SetActive(true);
                }
                else
                {
                    HermesRoomCoin.SetActive(false);
                }
            }
        }
        else if (collision.gameObject.tag == "NextHubArrow")
        {//Show an arrow to the next area
            NextHubArrow.SetActive(true);
            PlayerPrefs.SetFloat(PlayerHubPosition, -5f);
            PlayerPrefs.Save();
        }
        else if (collision.gameObject.tag == "PreviousHubArrow")
        {//Show an arrow to the next area
            PreviousHubArrow.SetActive(true);
            PlayerPrefs.SetFloat(PlayerHubPosition, 67f);
            PlayerPrefs.Save();
        }
        else if (collision.gameObject.tag == "PayHub")
        {//Let the player pay to access the next area
            PayText.SetActive(true);
            onPay = true;
        }
        else if (collision.gameObject.tag == "onBlue")
        {          
            onBlue = true;
        }
        else if (collision.gameObject.tag == "onRed")
        {
            onRed = true;
        }
        else if (collision.gameObject.tag == "onGreen")
        {
            onGreen = true;
        }
        else if (collision.gameObject.tag == "ZeusLetter")
        {//Show which letter the player is holding on to
            if (!aresLetter && !athenaLetter)
            {
                zeusLetter = true;
                ZeusLetterIcon.SetActive(true);
                AresLetterIcon.SetActive(false);
                AthenaLetterIcon.SetActive(false);
                Destroy(collision.gameObject);
            }
        }
        else if (collision.gameObject.tag == "AresLetter")
        {
            if(!zeusLetter && !athenaLetter)
            {
                aresLetter = true;
                ZeusLetterIcon.SetActive(false);
                AresLetterIcon.SetActive(true);
                AthenaLetterIcon.SetActive(false);
                Destroy(collision.gameObject);
            }
        }
        else if (collision.gameObject.tag == "AthenaLetter")
        {
            if (!zeusLetter && !aresLetter)
            {
                athenaLetter = true;
                ZeusLetterIcon.SetActive(false);
                AresLetterIcon.SetActive(false);
                AthenaLetterIcon.SetActive(true);
                Destroy(collision.gameObject);
            }
        }
        else if (collision.gameObject.tag == "Player")
        {
            if (sceneName == "HadesRoom" && this.gameObject.tag != "Chest")
            {
                Destroy(this.gameObject);
            }
            else if (sceneName == "ArtemisRoom")
            {//Show coin when player captures the boar
                coin.SetActive(true);
                Invoke("activateCoin", 2);
            }
            else
            {//Check if player in on the chest
                onChest = true;
            }
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        sceneToLoad = "";
        if (sceneName == "Hub")
        {//When out of range of a room there will be no room to load
            ZeusRoom.SetActive(false);
            PoseidonRoom.SetActive(false);
            HadesRoom.SetActive(false);

            ZeusRoomCoin.SetActive(false);
            PoseidonRoomCoin.SetActive(false);
            HadesRoomCoin.SetActive(false);

            PayText.SetActive(false);
            PreviousHubArrow.SetActive(false);
            NextHubArrow.SetActive(false);
            zeusRoom = false;
            poseidonRoom = false;
            hadesRoom = false;
        }
        else if(sceneName == "Hub2")
        {
            HeraRoom.SetActive(false);
            DemeterRoom.SetActive(false);
            AresRoom.SetActive(false);

            HeraRoomCoin.SetActive(false);
            DemeterRoomCoin.SetActive(false);
            AresRoomCoin.SetActive(false);

            PayText.SetActive(false);
            NextHubArrow.SetActive(false);
            heraRoom = false;
            demeterRoom = false;
            aresRoom = false;
        }
        else if (sceneName == "Hub3")
        {
            ApolloRoom.SetActive(false);
            ArtemisRoom.SetActive(false);
            AthenaRoom.SetActive(false);

            ApolloRoomCoin.SetActive(false);
            ArtemisRoomCoin.SetActive(false);
            AthenaRoomCoin.SetActive(false);

            PayText.SetActive(false);
            NextHubArrow.SetActive(false);
            PreviousHubArrow.SetActive(false);
            apolloRoom = false;
            artemisRoom = false;
            athenaRoom = false;
        }
        else if (sceneName == "Hub4")
        {
            HephaestusRoom.SetActive(false);
            AphroditeRoom.SetActive(false);
            HermesRoom.SetActive(false);

            HephaestusRoomCoin.SetActive(false);
            AphroditeRoomCoin.SetActive(false);
            HermesRoomCoin.SetActive(false);

            PayText.SetActive(false);
            NextHubArrow.SetActive(false);
            PreviousHubArrow.SetActive(false);
            hephaestusRoom = false;
            aphroditeRoom = false;
            hermesRoom = false;
        }
        onPay = false;
        onChest = false;
        onBlue = false;
        onRed = false;
        onGreen = false;
        submitLetter = false;
        submitWrongLetter = false;
    }
    void activateCoin()
    {
        coin.GetComponent<BoxCollider2D>().enabled = true;
    }
}
