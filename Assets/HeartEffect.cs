using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartEffect : MonoBehaviour
{
    public GameObject lifeEffect;
    [SerializeField] Player player;
    public void showLife()
    {//Play heart animation when collected
        Instantiate(lifeEffect, transform.position, Quaternion.identity);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {//Add 1 life to the player when collecting a heart
            player.GetComponent<Player>().heartGained();
            player.GetComponent<Player>().lives += 1;
            showLife();
            Destroy(this.gameObject);
        }
    }
}
