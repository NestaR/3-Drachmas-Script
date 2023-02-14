using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedusaController : MonoBehaviour
{
    public bool active, turnLeft;
    float activeTimer;
    public Transform playerDirection;
    public GameObject petrigaze, eye1, eye2;

    void Start()
    {
        deactivateEyes();
    }

    void Update()
    {
        activeTimer += Time.deltaTime;
        if(turnLeft)
        {
            if (!active)
            {
                if (activeTimer > 3f)
                {//Every 3 seconds the gameobject turns to face the player
                    this.transform.Rotate(0, 90, 0, Space.Self);
                    active = true;
                    activeTimer = 0;
                }
                else if (activeTimer > 2f)
                {
                    activateEyes();
                }
            }
            else
            {
                if (activeTimer > 3f)
                {
                    this.transform.Rotate(0, -90, 0, Space.Self);
                    active = false;
                    activeTimer = 0;
                    deactivateEyes();
                }
            }

            if (playerDirection.localScale.x > 0)
            {//Check which direction the player is facing
                //If this gameobject is facing the player it will deal damage
                petrigaze.GetComponent<PolygonCollider2D>().enabled = true;
            }
            else
            {//If the player is looking away from the gameobject nothing will happen
                petrigaze.GetComponent<PolygonCollider2D>().enabled = false;
            }
        }
        else
        {
            if (!active)
            {
                if (activeTimer > 3f)
                {
                    this.transform.Rotate(0, -90, 0, Space.Self);
                    active = true;
                    activeTimer = 0;
                }
                else if (activeTimer > 2f)
                {
                    activateEyes();
                }
            }
            else
            {
                if (activeTimer > 3f)
                {
                    this.transform.Rotate(0, 90, 0, Space.Self);
                    active = false;
                    activeTimer = 0;
                    deactivateEyes();
                }
            }
            if (playerDirection.localScale.x < 0)
            {
                petrigaze.GetComponent<PolygonCollider2D>().enabled = true;
            }
            else
            {
                petrigaze.GetComponent<PolygonCollider2D>().enabled = false;
            }
        }
    }
    void activateEyes()
    {//Change the colour of the gameobjects eyes
        var platRenderer = eye1.gameObject.GetComponent<Renderer>();
        platRenderer.material.SetColor("_Color", Color.red);
        var platRenderer2 = eye2.gameObject.GetComponent<Renderer>();
        platRenderer2.material.SetColor("_Color", Color.red);
    }
    void deactivateEyes()
    {//Revert the colour of the gameobjects eyes
        var platRenderer = eye1.gameObject.GetComponent<Renderer>();
        platRenderer.material.SetColor("_Color", Color.yellow);
        var platRenderer2 = eye2.gameObject.GetComponent<Renderer>();
        platRenderer2.material.SetColor("_Color", Color.yellow);
    }
}
