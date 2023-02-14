using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovementVertical: MonoBehaviour
{

    [SerializeField] float offsetBottom = 0, offsetTop = 5, speed = 1;
    [SerializeField] bool hasReachedTop = false, hasReachedBottom = false;
    Vector3 startPosition = Vector3.zero;
    public bool isTrigger = false, reset, playerOn, stopped, stepPlatform, moveRight = false, jumped, sun, door, elevator;
    public float fixedMove, fixedMoveBottom, lastOnPlatform;
    public Vector3 startPos, movePos;
    public GameObject newPlat, moveStepPlatform, sunBlock;
    void Awake()
    {//Store start position to reset platform
        startPosition = transform.position;
    }
    void Start()
    {
        playerOn = false;
        jumped = false;
        stopped = false;
        fixedMove = offsetTop;
        fixedMoveBottom = offsetBottom;
        startPos = transform.position;
    }
    void Update()
    {
        if(jumped)
        {
            Move(offsetTop);
            if(hasReachedTop)
            {
                offsetBottom = offsetTop;
                offsetTop += 5f;
                speed = 0;
                jumped = false;
                hasReachedBottom = true;
                stopped = false;
            }
        }
        else
        {
            lastOnPlatform = 0;
        }
        if(reset)
        {
            ResetPlatform();
            moveStepPlatform.GetComponent<PlatformMovementVertical>().ResetPlatform();
        }  
        if(sun)
        {//The sun object uses this script for Apollo room
            if (transform.position.y < ((startPosition.y + offsetTop) - 12f))
            {
                sunBlock.gameObject.SetActive(false);
            }
            else
            {
                sunBlock.gameObject.SetActive(true);
            }
            if(hasReachedTop)
            {
                speed = 0;
            }
        }

    }
    void FixedUpdate()
    {
        if (isTrigger && playerOn && stepPlatform)
        {//When the player is on a trigger platform it will move
            if(!stepPlatform)
            {

            }
            else
            {
                hasReachedTop = false;
                hasReachedBottom = true;
                if (!stopped)
                {//These platforms move when meeting certain conditions
                    if (!moveRight)
                    {
                        movePos = new Vector3(transform.position.x - 6, transform.position.y + 3.5f, transform.position.z);
                    }
                    else
                    {
                        movePos = new Vector3(transform.position.x + 6, transform.position.y + 3.5f, transform.position.z);
                    }
                    moveRight = !moveRight;
                    moveStepPlatform.GetComponent<PlatformMovementVertical>().newPlatform();
                    moveStepPlatform.GetComponent<PlatformMovementVertical>().jumped = true;
                    stopped = true;
                }
            }          
        }
        else if (isTrigger && !stepPlatform)
        {
            if(elevator)
            {//Stop the platform when reaching the top or bottom of its path
                if(hasReachedTop)
                {
                    offsetTop = offsetBottom;
                    if(!stopped)
                    {
                        this.gameObject.GetComponent<PlatformMovementVertical>().enabled = false;
                        stopped = true;
                    }                 
                }
                if(hasReachedBottom)
                {

                    ResetPlatform();
                }
            }
            else
            {
                if (hasReachedTop && !playerOn && !door)
                {//When the plat reaches its destination it will reset after 3 seconds
                    Invoke("ResetPlatform", 3f);
                }
            }

        }
        if (!hasReachedTop)
        {//Move the platform until it reaches its top location

            if (transform.position.y < startPosition.y + offsetTop)
            {
                Move(offsetTop);
            }
            else if (transform.position.y >= startPosition.y + offsetTop)
            {
                hasReachedTop = true;
                hasReachedBottom = false;
            }
        }
        else if (!hasReachedBottom)
        {//Move the platform until it reaches its bottom location
            if (transform.position.y > startPosition.y + offsetBottom)
            {
                Move(offsetBottom);
            }
            else if (transform.position.y <= startPosition.y + offsetBottom)
            {
                hasReachedTop = false;
                hasReachedBottom = true;
            }
        }

    }

    void Move(float offset)
    {//Move the platform by a specified distance vertically
        transform.position = Vector3.MoveTowards(transform.position,
                                                 new Vector3(transform.position.x,
                                                             startPosition.y + offset,
                                                             transform.position.z),
                                                 speed * Time.deltaTime);
    }
    public void ResetPlatform()
    {
        if(!elevator)
        {
            transform.position = startPosition;
            hasReachedTop = false;
        }      
        offsetTop = fixedMove;
        offsetBottom = fixedMoveBottom;
        isTrigger = false;
        stopped = false;
        jumped = false;
        reset = false;
        
        
        if(!stepPlatform)
        {
            hasReachedBottom = false;
            this.gameObject.GetComponent<PlatformMovementVertical>().enabled = false;
            
        }
        else
        {
            speed = 0;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {//Checks if the player is on the platform
            playerOn = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOn = false;
        }
    }
    public void newPlatform()
    {//Instantiate a new platform at its start position
        speed = 5;
    }
}
