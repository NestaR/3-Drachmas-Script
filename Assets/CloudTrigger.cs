using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {//Check if the cloud should be following the player or not (Zeus Room)
            if(GameObject.Find("Cloud 9").GetComponent<CloudController>().cloudTrigger)
            {
                GameObject.Find("Cloud 9").GetComponent<CloudController>().cloudTrigger = false;
                Destroy(this.gameObject);
            }
            else
            {
                GameObject.Find("Cloud 9").GetComponent<CloudController>().cloudTrigger = true;
                Destroy(this.gameObject);
            }
        }
    }
}
