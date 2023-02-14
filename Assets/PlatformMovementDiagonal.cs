using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovementDiagonal : MonoBehaviour
{

    [SerializeField] float offsetLeft = 5, offsetRight = 5, offsetTop = 5, offsetBottom = 5, speedHorizontal = 1, speedVertical = 1;
    [SerializeField] public bool hasReachedRight = false, hasReachedLeft = false, hasReachedTop = false, hasReachedBottom = false;
    Vector3 startPosition = Vector3.zero;
    public bool isTrigger = false, isBoat, waiting, hasPlayer;
    public GameObject[] bounds;
    public GameObject newPlat;
    private float waitTimer;
    public int stage = 1;
    public int numOfEnemies;
    float defaultTop, defaultBottom, defaultLeft, defaultRight;
    private const string PlayerDeathZone = "PlayerDeathZone";
    void Awake()
    {
        startPosition = transform.position;

    }
    public void SetInt(string KeyName, int Value)
    {
        PlayerPrefs.SetInt(KeyName, Value);
    }
    public int GetInt(string KeyName)
    {
        return PlayerPrefs.GetInt(KeyName);
    }
    void Start()
    {
        defaultTop = offsetTop;
        defaultBottom = offsetBottom;
        defaultLeft = offsetLeft;
        defaultRight = offsetRight;
    }
    void Update()
    {
        PlayerPrefs.GetInt(PlayerDeathZone);
        if (PlayerPrefs.GetInt(PlayerDeathZone) == 1)
        {
            Invoke("resetPlatform", 1f);
            
        }
    }
    void FixedUpdate()
    {       
        if (isTrigger)
        {
            if (isBoat)
            {
                foreach (GameObject bound in bounds)
                {
                    bound.SetActive(true);
                    bound.GetComponent<BoxCollider2D>().isTrigger = false;
                }
                if(offsetRight > 0)
                {//Stop the platform when reaching the end of its path
                    if (hasReachedRight)
                    {
                        offsetLeft = offsetRight;
                        offsetBottom = 0;
                        offsetTop = 0;
                        isTrigger = false;
                        hasPlayer = false;
                    }
                }
                else
                {
                    if (hasReachedLeft)
                    {
                        offsetRight = offsetLeft;
                        offsetBottom = 0;
                        offsetTop = 0;
                        isTrigger = false;
                        hasPlayer = false;
                    }
                }

            }
            else
            {
                if (hasReachedRight && !isTrigger)
                {
                    Invoke("newPlatform", 3f);
                }
                else if(hasReachedRight)
                {
                    offsetLeft = offsetRight;
                    offsetBottom = defaultBottom;
                    offsetTop = defaultTop;
                }
            }
        }
        else
        {
            if (isBoat)
            {
                foreach (GameObject bound in bounds)
                {
                    bound.SetActive(false);
                    bound.GetComponent<BoxCollider2D>().isTrigger = true;
                }
                if(waiting)
                {
                    Invoke("resetPlatform", 5f);
                }
            }
        }
        if (!hasReachedRight)
        {//Diagonal platforms move vertically and horizontally at the same time
            if (transform.position.x < startPosition.x + offsetRight)
            {
                Move(offsetRight);
            }
            else if (transform.position.x >= startPosition.x + offsetRight)
            {             
                hasReachedRight = true;
                hasReachedLeft = false;
            }
        }
        else if (!hasReachedLeft)
        {
            if (transform.position.x > startPosition.x + offsetLeft)
            {
                Move(offsetLeft);
            }
            else if (transform.position.x <= startPosition.x + offsetLeft)
            {
                hasReachedRight = false;
                hasReachedLeft = true;
            }
        }
        if (!hasReachedTop)
        {
            if (transform.position.y < startPosition.y + offsetTop)
            {
                MoveDiag(offsetTop);
            }
            else if (transform.position.y >= startPosition.y + offsetTop)
            {
                hasReachedTop = true;
                hasReachedBottom = false;
            }
        }
        else if (!hasReachedBottom)
        {
            if (transform.position.y > startPosition.y + offsetBottom)
            {
                MoveDiag(offsetBottom);
            }
            else if (transform.position.y <= startPosition.y + offsetBottom)
            {
                hasReachedTop = false;
                hasReachedBottom = true;
            }
        }

    }

    void Move(float offset)
    {
        transform.position = Vector3.MoveTowards(transform.position,
                                                    new Vector3(startPosition.x + offset,
                                                                transform.position.y,
                                                                transform.position.z),
                                                    speedHorizontal * Time.deltaTime);
    }
    void MoveDiag(float offset)
    {
        transform.position = Vector3.MoveTowards(transform.position,
                                         new Vector3(transform.position.x,
                                                     startPosition.y + offset,
                                                     transform.position.z),
                                         speedVertical * Time.deltaTime);
    }
    public void newPlatform()
    {
        hasReachedRight = false;
        Instantiate(newPlat, startPosition, Quaternion.identity);
        Destroy(this.gameObject);
    }
    public void resetPlatform()
    {
        hasReachedLeft = false;
        hasReachedRight = false;
        hasReachedTop = false;
        hasReachedBottom = false;
        transform.position = startPosition;
        offsetLeft = defaultLeft;
        offsetRight = defaultRight;
        offsetBottom = defaultBottom;
        offsetTop = defaultTop;
        isTrigger = false;
        this.GetComponent<PlatformMovementDiagonal>().enabled = false;
        PlayerPrefs.SetInt(PlayerDeathZone, 0);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            numOfEnemies += 1;
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            hasPlayer = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            numOfEnemies -= 1;
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            hasPlayer = false;
        }
    }

}

