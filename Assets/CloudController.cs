using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour
{
    public Transform player;
    public Vector3 offset, playerPos;
    private float followTimer, chargeTimer;
    public GameObject lightningbolt;
    BoxCollider2D boltCollider;
    public bool cloudTrigger, lSound;
    AudioSource source;
    public AudioClip lightningSound, thunderSound;

    void Start()
    {
        boltCollider = GetComponent<BoxCollider2D>();
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(cloudTrigger)
        {
            followTimer += Time.deltaTime;

            if (followTimer <= 4f)
            {//Follow the player for a certain time
                var platRenderer = this.gameObject.GetComponent<Renderer>();
                platRenderer.material.SetColor("_Color", Color.white);
                lightningbolt.GetComponent<LightningBoltScript>().ManualMode = true;
                boltCollider.enabled = false;
                lSound = false;
                playerPos = new Vector3(player.position.x + offset.x, player.position.y + offset.y, player.position.z + offset.z);
                if (playerPos.y <= 7)
                {//Keep the cloud a certain distance from the player
                    playerPos.y = 7;
                }
                else if (playerPos.y >= 14)
                {
                    playerPos.y = 14;
                }
                transform.position = Vector3.Slerp(transform.position, playerPos, Time.deltaTime * 3f);
            }
            else
            {//Stationary clouds will periodically shoot lightning
                chargeTimer += Time.deltaTime;
                if (chargeTimer >= 3.3f)
                {
                    followTimer = 0f;
                    chargeTimer = 0f;
                }
                else if (chargeTimer >= 1.5f)
                {
                    lightningbolt.GetComponent<LightningBoltScript>().ManualMode = false;
                    boltCollider.enabled = true;
                    source.Stop();
                    if(lSound)
                    {
                        playLSound();
                    }                
                }
                else if(chargeTimer >= 0.7f && chargeTimer <= 1f)
                {//Play a sound before the attack
                    lSound = true;
                }
                else if(chargeTimer <= 0.3f)
                {
                    if (!source.isPlaying)
                    {
                        source.Play();
                    }
                    var platRenderer = this.gameObject.GetComponent<Renderer>();
                    platRenderer.material.SetColor("_Color", Color.grey);
                }
            }
        }
        else
        {
            if (lightningbolt.GetComponent<LightningBoltScript>().charging)
            {//Enable the hit collider when shooting lightning
                boltCollider.enabled = false;
                var platRenderer = this.gameObject.GetComponent<Renderer>();
                platRenderer.material.SetColor("_Color", Color.white);
                if (!source.isPlaying)
                {
                    source.Play();
                }
            }
            else
            {
                var platRenderer = this.gameObject.GetComponent<Renderer>();
                platRenderer.material.SetColor("_Color", Color.grey);
                source.Stop();
                boltCollider.enabled = true;
            }
        }
    }
    public void playLSound()
    {
        source.PlayOneShot(lightningSound, 1);
        lSound = false;
    }
}
