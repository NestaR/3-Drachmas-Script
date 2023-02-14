using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonScript : MonoBehaviour
{
    [SerializeField] private LayerMask platformLayer;
    private Transform platform;
    private bool isOnThePlatform = false;

    private Animator animator;
    private Rigidbody2D _rigidbody;
    public bool IsFacingRight { get; private set; }
    private GameObject player, enemyChild;
    private Vector3 newPos, targetPos;
    public float offset = 0f;
    private float attackTimer, shieldTimer;
    public bool attacking, hit, canShield, canAttack, shielding, dead, hitting;
    public float attackX, attackY;
    private Vector2 currentOffset;
    CircleCollider2D enemyCollider;
    public int hitCount;
    PlatformMovementHorizontal pmh;
    AudioSource source;
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        currentOffset = this.GetComponent<BoxCollider2D>().offset;
        animator = GetComponent<Animator>();
        player = GameObject.Find("Player");
        GameObject ChildGameObject1 = this.transform.GetChild(0).gameObject;
        enemyCollider = ChildGameObject1.GetComponent<CircleCollider2D>();
    }
    void Update()
    {
        if (platform != null) transform.parent = platform;
        else transform.parent = null;
        if (hitCount >= 2)
        {//If the skeleton was hit twice it dies
            enemyCollider.enabled = false;
            animator.SetTrigger("Death");
            Invoke("showDeath", 1);
            _rigidbody.gravityScale = 0f;
            this.GetComponent<BoxCollider2D>().enabled = false;
        }
        else
        {
            //The skeleton will follow the player until it reaches striking distance
            if (Vector3.Distance(transform.position, player.transform.position) >= 2.3f)
            {
                if(!attacking && !shielding && attackTimer <= 0)
                {
                    MoveTowardsTarget();
                }                       
            }
            else
            {//When the skeleton is within striking distance it can attack
                animator.SetBool("Walk", false);
                canAttack = true;
            }

            if (canAttack)
            {
                attackTimer += Time.deltaTime;
            }
            if (attackTimer >= 0.7f)
            {
                attacking = true;
            }
            else
            {
                attacking = false;
            }

            if (attacking && !hit && !shielding && canAttack)
            {
                if (attackTimer >= 2f)
                {//End of skeletons hit animation
                    animator.SetBool("Attack1", false);
                    attacking = false;
                    canAttack = false;
                    attackTimer = 0f;
                    //shieldTimer = 0;
                    //canShield = true;
                    enemyCollider.enabled = false;
                }
                else if (attackTimer >= 1.4f)
                {//The skeletons hit collider is enabled
                 
                    enemyCollider.enabled = true;
                }
                else if (attackTimer >= 1f)
                {//Start the attack animation
                    animator.SetBool("Attack1", true);
                }

            }

            //if (canShield)
            //{
            //    canAttack = false;
            //    shieldTimer += Time.deltaTime;
            //    if (shieldTimer >= 1f)
            //    {
            //        shielding = true;                    
            //    }
            //    else
            //    {
            //        shielding = false; 
            //    }
            //}         
            //if (shielding && canShield)
            //{
            //    attacking = false;
            //    animator.SetBool("Shield", true);
            //    enemyCollider.enabled = false;
            //    if (shieldTimer >= 2.5f)
            //    {
            //        shielding = false;
            //        animator.SetBool("Shield", false);
            //        canAttack = true;
            //        canShield = false;
            //        attackTimer = 0f;
            //        shieldTimer = 0;
            //    }
            //}

            targetPos = player.transform.position - transform.position;
            CheckDirectionToFace(targetPos.x < 0f);
        }


    }
    void LateUpdate()
    {
        if (hit)
        {
            hitCount += 1;
            hit = false;
        }
    }
    public void SetGravityScale(float scale)
    {
        _rigidbody.gravityScale = scale;
    }
    private void Turn()
    {
        //stores scale and flips the player along the x axis, 
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        IsFacingRight = !IsFacingRight;
    }
    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
            Turn();
    }
    private void MoveTowardsTarget()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, 2f * Time.deltaTime);
        animator.SetBool("Walk", true);
    }

    private void RotateTowardsTarget()
    {
        Vector2 direction = player.transform.position - transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(0f, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(Vector3.forward * (angle + offset));
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //If the object hit is a platform it will become its child
        if ((collision.gameObject.CompareTag("MPH") || collision.gameObject.CompareTag("MPV")) && isOnThePlatform)
        {
            isOnThePlatform = true;
        }
        if (collision.gameObject.CompareTag("MPH") || collision.gameObject.CompareTag("MPV"))
        {
            this.platform = collision.gameObject.transform;

        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerHit") && !shielding)
        {//If the skeleton is hit it loses a life
            Debug.Log("Skeleton Hit");
            animator.SetTrigger("Hit");
            hit = true;
            hitSound();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //hit = false;
        attackTimer = 0;
    }
    public void showDeath()
    {
        Destroy(this.gameObject);
    }
    public void hitSound()
    {
        source.Play();
    }
}
