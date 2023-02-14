using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TridentScript : MonoBehaviour
{
    public GameObject trident, player, pointer;
    public float tridentTimer, yoffset, fireTimer = 1f, waitTimer = 2f;
    Vector2 playerPos;
    Vector3 launch;
    public bool isTrident, stationary, aimLeft, aimRight, fired;

    void OnEnable()
    {
        fired = false;
        tridentTimer = 0;
    }
    void Update()
    {
        if (!isTrident)
        {//The trident shooter object will follow the player until its ready to fire
            tridentTimer += Time.deltaTime;
            if (!stationary)
            {
                if (tridentTimer >= fireTimer)
                {//Instantiate a trident prefab at the current position
                    playerPos.x = transform.position.x;
                    playerPos.y = player.transform.position.y + yoffset;
                    Instantiate(trident, playerPos, transform.rotation);
                    launch = new Vector3(transform.position.x - 25, transform.position.y, transform.position.z);
                    tridentTimer = 0;
                }
                else if (tridentTimer >= (fireTimer - 1f))
                {//Change the colour to alert the player
                    var pointerRenderer = pointer.GetComponent<Renderer>();
                    pointerRenderer.material.SetColor("_Color", Color.red);
                }
                else
                {
                    var pointerRenderer = pointer.GetComponent<Renderer>();
                    pointerRenderer.material.SetColor("_Color", Color.yellow);
                }
                transform.position = new Vector3(player.transform.position.x + 31.8f, player.transform.position.y + yoffset, transform.position.z);
                pointer.transform.position = new Vector3(pointer.transform.position.x, player.transform.position.y + yoffset, pointer.transform.position.z);
            }
            else
            {

                if (tridentTimer >= fireTimer && !fired)
                {
                    playerPos.x = transform.position.x;
                    playerPos.y = transform.position.y;
                    Instantiate(trident, playerPos, transform.rotation);
                    fired = true;
                }
                else if (tridentTimer >= fireTimer + waitTimer)
                {
                    tridentTimer = 0;
                    var pointerRenderer = pointer.GetComponent<Renderer>();
                    pointerRenderer.material.SetColor("_Color", Color.yellow);
                    fired = false;
                }
                else if (tridentTimer >= (fireTimer * 0.7f))
                {
                    var pointerRenderer = pointer.GetComponent<Renderer>();
                    pointerRenderer.material.SetColor("_Color", Color.red);
                }
            }
        }
        else
        {
            tridentTimer += Time.deltaTime;
            if (stationary && aimRight)
            {
                launch = new Vector3(transform.position.x + 33, transform.position.y, transform.position.z);
                transform.position = Vector3.Lerp(transform.position, launch, Time.deltaTime * 0.8f);
            }
            else if (stationary && !aimLeft && !aimRight)
            {//Stationary tridents will go in a straight line for a specific distance
                launch = new Vector3(transform.position.x, transform.position.y + 40, transform.position.z);
                transform.position = Vector3.Lerp(transform.position, launch, Time.deltaTime * 0.9f);
            }
            else if(stationary && aimLeft)
            {
                launch = new Vector3(transform.position.x - 25, transform.position.y, transform.position.z);
                transform.position = Vector3.Lerp(transform.position, launch, Time.deltaTime * 0.7f);
            }
            else
            {//Trident prefab will travel to behind the players position
                launch = new Vector3(transform.position.x - 25, transform.position.y, transform.position.z);
                transform.position = Vector3.Lerp(transform.position, launch, Time.deltaTime * 0.7f);
            }
            if (tridentTimer >= 2f)
            {//After 2 seconds destroy the trident prefab
                Destroy(this.gameObject);
                tridentTimer = 0;
            }
        }
    }
}
