using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    Vector3 pos;
    private Rigidbody2D _rigidbody;
    public GameObject endPrefab, deathPrefab, stageActivate, stageActivate2, hammerHead, hammerHandle, hepGlass, gameSound, worldCanvas;
    private Vector3 respawnPoint;
    private string sceneName;
    private Image life1, life2, life3;
    public Text coinText;
    public int lives = 3, numCoins, hammerPieces, jumpCounter;
    private bool death, trapped, oneshotDeath, oneshotEnd;
    public bool attacking, onVine, playerClimb, hasHammerHead, hasHammerHandle;
    private Animator animator;
    private bool playerHit, deathZoneHit;
    private float hitTimer, deathTimer, attackTimer;
    //[SerializeField] private int 
    private const string PlayerHubPosition = "PlayerHubPosition", PlayerCoins = "PlayerCoins";
    private const string ZeusCoin = "ZeusCoin", PoseidonCoin = "PoseidonCoin", HadesCoin = "HadesCoin", HeraCoin = "HeraCoin", DemeterCoin = "DemeterCoin", AresCoin = "AresCoin";
    private const string ApolloCoin = "ApolloCoin", ArtemisCoin = "ArtemisCoin", AthenaCoin = "AthenaCoin", HephaestusCoin = "HephaestusCoin", AphroditeCoin = "AphroditeCoin", HermesCoin = "HermesCoin";
    private const string PlayerDeathZone = "PlayerDeathZone";
    AudioSource source;
    public AudioClip lightningSound, deathSound, levelEndSound, buttonConfirm, playerhitSound, climbingSound, coinSpendSound, healSound, jumpSound, playerDeniedSound, forgeSound;
    public SpriteRenderer spriteRenderer;
    public Sprite climbingIdleSprite;
    private UnityEngine.AI.NavMeshAgent agent;
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    void OnEnable()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }
    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        Scene scene = SceneManager.GetActiveScene();
        sceneName = scene.name;
        respawnPoint = transform.position;
        numCoins = PlayerPrefs.GetInt(PlayerCoins);
        if (sceneName != "Hub" && sceneName != "Hub2" && sceneName != "Hub3" && sceneName != "Hub4")
        {//Show lives if not in hub
            Time.timeScale = 0;
            life1 = GameObject.Find("Life1").GetComponent<Image>();
            life2 = GameObject.Find("Life2").GetComponent<Image>();
            life3 = GameObject.Find("Life3").GetComponent<Image>();
        }
        else
        {
            Time.timeScale = 1;
            if (PlayerPrefs.HasKey(PlayerHubPosition))
            {//If the player has played before it will spawn them in their previous location
                transform.position = new Vector3(PlayerPrefs.GetFloat(PlayerHubPosition), transform.position.y, transform.position.z);
            }
            else
            {//Else it will spawn them at the beggining
                transform.position = new Vector3(-5f, transform.position.y, transform.position.z);
            }
            setCoins(numCoins);
        }
        //Show number of coins player has collected
        //Debug.Log(PlayerPrefs.GetInt(PlayerCoins));
    }
    public void SetInt(string KeyName, int Value)
    {
        PlayerPrefs.SetInt(KeyName, Value);
    }
    public int GetInt(string KeyName)
    {
        return PlayerPrefs.GetInt(KeyName);
    }

    public float GetFloat(string KeyName)
    {
        return PlayerPrefs.GetFloat(KeyName);
    }
    public void HasKey(string KeyName)
    {
        if (PlayerPrefs.HasKey(KeyName))
        {
            Debug.Log("The key " + KeyName + " exists");
        }
        else
        { 
            Debug.Log("The key " + KeyName + " does not exist");
        }
    }
    void Update()
    {
        PlayerPrefs.HasKey("PlayerCoins");
        if(hasHammerHead)
        {//Controller for Hephaestus room objects
            Vector2 followPlayer = new Vector2(transform.position.x, (transform.position.y + 3f));
            hammerHead.transform.position = Vector2.Lerp(hammerHead.transform.position, followPlayer, Time.deltaTime * 3f);
        }
        else if(hasHammerHandle)
        {
            Vector2 followPlayer = new Vector2(transform.position.x, (transform.position.y + 3f));
            hammerHandle.transform.position = Vector2.Lerp(hammerHandle.transform.position, followPlayer, Time.deltaTime * 3f);
        }
        if (hammerPieces == 5)
        {
            source.PlayOneShot(forgeSound, 1f);
            hammerPieces = 6;
            hammerHead.transform.position = new Vector2(-6.55f, -62.8f);
            hammerHandle.transform.position = new Vector2(-6.55f, -67f);
            Invoke("movePlayer", 2);
        }
        else if (hammerPieces == 2)
        {
            source.PlayOneShot(forgeSound, 1f);
            hammerPieces = 3;
            hammerHead.transform.position = new Vector2(-6.55f, 6.2f);
            hammerHandle.transform.position = new Vector2(-6.55f, 2f);
            Invoke("moveHammer", 2);
        }
        if (death)
        {//If the player dies an animation will play and the player will have to choose what to do next
            animator.SetBool("Death", true);
            this.GetComponent<Character2DController>().enabled = false;
            deathTimer += Time.deltaTime;
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            Invoke("showDeath", 2);
            Invoke("playerDeathSound", 0f);
            death = false;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown("w"))
        {
            if (onVine && !playerClimb)
            {//The player will hold on to a vine/ladder when interacting on it
                playerVineConnect();
            }              
        }

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey("w"))
        {
            if (playerClimb)
            {//The player will go up when holding onto a vine/ladder
                transform.position = transform.position + new Vector3(0f, 0.3f, 0f);
                animator.SetTrigger("Climbing");
            }

        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey("s"))
        {
            if (playerClimb)
            {//The player will go down when holding onto a vine/ladder
                transform.position = transform.position + new Vector3(0f, -0.3f, 0f);
                animator.SetTrigger("Climbing");
            }

        }
        if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp("w") || Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp("s"))
        {//Stop the players animation when not moving on a vine
            animator.ResetTrigger("Climbing");
        }
        if (playerHit)
        {//Disable the players collider when hit to prevent multiple hits     
            animator.SetBool("Jump", false);
            this.GetComponent<Character2DController>().isJumping = false;
            hitTimer += Time.deltaTime;
            this.GetComponent<Character2DController>().enabled = false;
        }
        if(!trapped && sceneName == "ArtemisRoom")
        {//Hide hint when hitting a trap
            worldCanvas.gameObject.SetActive(false);
        }
        if (hitTimer > 1f && !trapped)
        {//Player freezes and loses a life when hit
            animator.SetBool("Hit", false);
            lives -= 1;
            hitTimer = 0f;
            playerHit = false;
            _rigidbody.constraints = RigidbodyConstraints2D.None;
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            if(this.GetComponent<Character2DController>().playerOnIce)
            {
                _rigidbody.mass = 1.5f;
            }
            else
            {
                _rigidbody.mass = 1f;
            }
            GetComponent<Rigidbody2D>().gravityScale = 1.3f;
            _rigidbody.drag = 0;
            if (deathZoneHit)
            {
                transform.position = respawnPoint;
                deathZoneHit = false;
            }
        }
        else if(trapped)
        {//Show hint when hitting a trap
            worldCanvas.gameObject.SetActive(true);
            if (Input.GetButtonUp("Jump"))
            {
                jumpCounter += 1;
            }
            if(jumpCounter >= 3)
            {//Break free from the trap after jumping a few times
                jumpCounter = 0;
                trapped = false;
            }
        }

        if (sceneName != "Hub" && sceneName != "Hub2" && sceneName != "Hub3" && sceneName != "Hub4")
        {//When the player would lose a life it is represented on the UI
            if (lives <= 0)
            {
                death = true;
                life1.enabled = false;
                life2.enabled = false;
                life3.enabled = false;
            }
            else if (lives == 1)
            {
                life2.enabled = false;
                life3.enabled = false;
            }
            else if (lives == 2)
            {
                life2.enabled = true;
                life3.enabled = false;
            }
            else if (lives >= 3)
            {
                lives = 3;
                life3.enabled = true;
            }
        }
        else
        {
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DeathZone") && !deathZoneHit)
        {//If the player collides with the death zone it will teleport them to a previous checkpoint         
            deathZoneHit = true;
            if(sceneName == "ZeusRoom")
            {
                source.PlayOneShot(lightningSound, 0.1f);
            }
            PlayerPrefs.SetInt(PlayerDeathZone, 1);
            playerHitAnim();

        }
        else if (collision.gameObject.tag == "Respawn")
        {//Set a new checkpoint
            respawnPoint = transform.position;
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "StageSet")
        {//Used for hiding/showing stages of the level to improve optimisation
            collision.gameObject.SetActive(false);
            stageActivate.SetActive(true);
        }
        else if (collision.gameObject.tag == "StageSet2")
        {//Used for hiding/showing stages of the level to improve optimisation
            collision.gameObject.SetActive(false);
            stageActivate.SetActive(false);
            stageActivate2.SetActive(true);
        }
        else if (collision.gameObject.tag == "FTrigger")
        {//Shoots a fireball when triggered
            GameObject fireballtrigger = collision.gameObject;
            collision.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
        else if (collision.gameObject.tag == "Bolt")
        {//Trigger for lightning bolt hit
            playerHitAnim();
            source.PlayOneShot(lightningSound, 0.1f);
        }
        else if (collision.gameObject.tag == "Trap")
        {//Trigger for lightning bolt hit
            playerHitAnim();
            trapped = true;
            collision.GetComponent<SpriteRenderer>().enabled = true;
            collision.GetComponent<PolygonCollider2D>().enabled = false;
        }
        else if (collision.gameObject.tag == "Vine")
        {//Let the player hold onto the vine
            onVine = true;

        }
        else if (collision.gameObject.tag == "Coin")
        {//When the player collects a coin that information is stored
            Destroy(collision.gameObject);
            Invoke("showEnd", 1);
            gameSound.gameObject.SetActive(false);
            if (sceneName == "ZeusRoom")
            {
                if(PlayerPrefs.HasKey(ZeusCoin))
                {
                    Debug.Log("Coin in this room has been collected");
                }
                else
                {
                    PlayerPrefs.SetInt(ZeusCoin, 1);
                    numCoins += 1;
                }

            }
            else if (sceneName == "PoseidonRoom")
            {
                if (PlayerPrefs.HasKey(PoseidonCoin))
                {
                    Debug.Log("Coin in this room has been collected");
                }
                else
                {
                    PlayerPrefs.SetInt(PoseidonCoin, 1);
                    numCoins += 1;
                }
            }
            else if (sceneName == "HadesRoom")
            {
                if (PlayerPrefs.HasKey(HadesCoin))
                {
                    Debug.Log("Coin in this room has been collected");
                }
                else
                {
                    PlayerPrefs.SetInt(HadesCoin, 1);
                    numCoins += 1;
                }
            }
            else if (sceneName == "HeraRoom")
            {
                if (PlayerPrefs.HasKey(HeraCoin))
                {
                    Debug.Log("Coin in this room has been collected");
                }
                else
                {
                    PlayerPrefs.SetInt(HeraCoin, 1);
                    numCoins += 1;
                }

            }
            else if (sceneName == "DemeterRoom")
            {
                if (PlayerPrefs.HasKey(DemeterCoin))
                {
                    Debug.Log("Coin in this room has been collected");
                }
                else
                {
                    PlayerPrefs.SetInt(DemeterCoin, 1);
                    numCoins += 1;
                }
            }
            else if (sceneName == "AresRoom")
            {
                if (PlayerPrefs.HasKey(AresCoin))
                {
                    Debug.Log("Coin in this room has been collected");
                }
                else
                {
                    PlayerPrefs.SetInt(AresCoin, 1);
                    numCoins += 1;
                }
            }
            else if (sceneName == "ApolloRoom")
            {
                if (PlayerPrefs.HasKey(ApolloCoin))
                {
                    Debug.Log("Coin in this room has been collected");
                }
                else
                {
                    PlayerPrefs.SetInt(ApolloCoin, 1);
                    numCoins += 1;
                }

            }
            else if (sceneName == "ArtemisRoom")
            {
                if (PlayerPrefs.HasKey(ArtemisCoin))
                {
                    Debug.Log("Coin in this room has been collected");
                }
                else
                {
                    PlayerPrefs.SetInt(ArtemisCoin, 1);
                    numCoins += 1;
                }
            }
            else if (sceneName == "AthenaRoom")
            {
                if (PlayerPrefs.HasKey(AthenaCoin))
                {
                    Debug.Log("Coin in this room has been collected");
                }
                else
                {
                    PlayerPrefs.SetInt(AthenaCoin, 1);
                    numCoins += 1;
                }
            }
            else if (sceneName == "HephaestusRoom")
            {
                if (PlayerPrefs.HasKey(HephaestusCoin))
                {
                    Debug.Log("Coin in this room has been collected");
                }
                else
                {
                    PlayerPrefs.SetInt(HephaestusCoin, 1);
                    numCoins += 1;
                }

            }
            else if (sceneName == "AphroditeRoom")
            {
                if (PlayerPrefs.HasKey(AphroditeCoin))
                {
                    Debug.Log("Coin in this room has been collected");
                }
                else
                {
                    PlayerPrefs.SetInt(AphroditeCoin, 1);
                    numCoins += 1;
                }
            }
            else if (sceneName == "HermesRoom")
            {
                if (PlayerPrefs.HasKey(HermesCoin))
                {
                    Debug.Log("Coin in this room has been collected");
                }
                else
                {
                    PlayerPrefs.SetInt(HermesCoin, 1);
                    numCoins += 1;
                }
            }
            PlayerPrefs.SetInt("PlayerCoins", numCoins);
            PlayerPrefs.Save();
        }
        else if (collision.gameObject.tag == "EnemyHit")
        {//When player is hit play animation and lose a life
            if(!playerHit)
            {
                playerHitAnim();
            }
            
        }
        else if (collision.gameObject.tag == "Trident")
        {//When player is hit by a trident they will lose a life
            playerHit = true;
            source.PlayOneShot(playerhitSound, 1f);
            animator.SetBool("Hit", true);
            this.GetComponent<Character2DController>().crouching = false;
            this.GetComponent<Character2DController>().sliding = false;
            this.GetComponent<Character2DController>().isJumping = false;
        }
        else if (collision.gameObject.tag == "Boar")
        {//When player catches a boar it will stop and give the coin
            collision.GetComponent<PlatformMovementHorizontal>().boarCaught = true;
        }
        else if (collision.gameObject.tag == "HammerHead" && !hasHammerHandle)
        {//Carry one of the hammer pieces
            hasHammerHead = true;
        }
        else if (collision.gameObject.tag == "HammerHandle" && !hasHammerHead)
        {//Carry one of the hammer pieces
            hasHammerHandle = true;
        }
        else if (collision.gameObject.tag == "HammerTrigger")
        {//Place one of the hammer pieces
            if(hasHammerHead)
            {
                hasHammerHead = false;
                hammerHead.GetComponent<BoxCollider2D>().enabled = false;
                hammerPieces += 1;
            }
            else if(hasHammerHandle)
            {
                hasHammerHandle = false;
                hammerHandle.GetComponent<BoxCollider2D>().enabled = false;
                hammerPieces += 1;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Vine")
        {
            onVine = false;
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            playerClimb = false;
            animator.SetBool("OnLadder", false);
        }
        else if (collision.gameObject.tag == "FTrigger")
        {//Used for hiding/showing stages of the level to improve optimisation
            GameObject fireballtrigger = collision.gameObject;
            collision.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    void playerHitAnim()
    {//When player is hit prevent movement and stop animations
        source.PlayOneShot(playerhitSound, 1f);
        GetComponent<Rigidbody2D>().gravityScale = 0;
        playerHit = true;
        animator.SetBool("Hit", true);
        animator.SetBool("Jump", false);
        animator.SetBool("Crouching", false);
        animator.SetBool("Sliding", false);
        this.GetComponent<Character2DController>().crouching = false;
        this.GetComponent<Character2DController>().sliding = false;
        _rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX;
        _rigidbody.constraints = RigidbodyConstraints2D.FreezePositionY;
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        _rigidbody.mass = 1000f;
        _rigidbody.drag = 20;
    }
    public void playerVineConnect()
    {
        source.PlayOneShot(climbingSound, 1f);
        playerClimb = true;
        animator.SetBool("OnLadder", true);
        _rigidbody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        this.GetComponent<Character2DController>().isJumping = false;
        this.GetComponent<Character2DController>().isJumpCut = false;
        this.GetComponent<Character2DController>().jumpReturned = true;
        //spriteRenderer.sprite = climbingIdleSprite;

    }
    public void showDeath()
    {//Show players death animation when dying
        this.gameObject.SetActive(false);
        deathPrefab.SetActive(true);      
    }
    public void playerDeathSound()
    {
        if(!oneshotDeath)
        {
            gameSound.gameObject.SetActive(false);
            oneshotDeath = true;
            source.PlayOneShot(deathSound, 1f);
        }
    }
    public void coinSound()
    {
        source.PlayOneShot(coinSpendSound, 1f);
    }
    public void deniedSound()
    {
        source.PlayOneShot(playerDeniedSound, 1f);
    }
    public void moveHammer()
    {
        hepGlass.GetComponent<BoxCollider2D>().isTrigger = true;
        hammerHead.transform.position = new Vector2(-15.55f, -47f);
        hammerHandle.transform.position = new Vector2(2.55f, -47f);
        hammerHead.GetComponent<BoxCollider2D>().enabled = true;
        hammerHandle.GetComponent<BoxCollider2D>().enabled = true;
    }
    public void movePlayer()
    {
        transform.position = new Vector2(-6.55f, 72f);
        hammerPieces = 0;
    }
    public void heartGained()
    {
        source.PlayOneShot(healSound, 1f);
    }
    public void jumpSoundPlay()
    {
        source.PlayOneShot(jumpSound, 1f);
    }
    void showEnd()
    {
        this.GetComponent<Character2DController>().enabled = false;
        endPrefab.SetActive(true);
        Time.timeScale = 0;
        source.volume = 0f;
    }
    public void setCoins(int coins)
    {
        coinText.text = coins.ToString();
    }
    public void buttonConfirmSound()
    {
        source.PlayOneShot(buttonConfirm, 1f);
    }
    IEnumerator LoadYourAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("EndScene");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
