using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surfmode : MonoBehaviour
{
    public GameObject tridentshooter1, tridentshooter2, tridentshooter3;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {//Activate each of the trident shooters when the player reaches a certain part of the level (Poseidon Room)
            if (tridentshooter3.activeSelf)
            {
                tridentshooter1.SetActive(false);
                tridentshooter2.SetActive(false);
                tridentshooter3.SetActive(false);
                Destroy(this.gameObject);
            }
            else if (tridentshooter2.activeSelf)
            {
                tridentshooter3.SetActive(true);
                Destroy(this.gameObject);
            }
            else if (tridentshooter1.activeSelf)
            {
                tridentshooter1.SetActive(false);
                tridentshooter2.SetActive(true);
                Destroy(this.gameObject);
            }
            else if(!tridentshooter1.activeSelf && !tridentshooter2.activeSelf)
            {
                tridentshooter1.SetActive(true);
                Destroy(this.gameObject);
            }
        }
    }
}
