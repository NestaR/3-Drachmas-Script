using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TrapScript : MonoBehaviour
{
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {//Play an animation when the player triggers the trap
            animator.SetTrigger("TrapHit");
        }
    }
}
