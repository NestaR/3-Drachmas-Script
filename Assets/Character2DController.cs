using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;
using UnityEngine.SceneManagement;

public class Character2DController : MonoBehaviour
{
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask climbingLayer;
    private Transform platform;
    private string sceneName;
    private bool isOnThePlatform = false;
    CircleCollider2D circleCollider;
    private Rigidbody2D _rigidbody;
    private Animator animator;
    private Vector2 _movement;
    public float LastOnGroundTime;
    public bool IsFacingRight { get; private set; }
    public bool jumpReturned = true;
    //[SerializeField] private float rotationSpeed;
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.7f, 0.03f);

    public float runMaxSpeed; //Target speed we want the player to reach.
    public float runAcceleration; //Time (approx.) time we want it to take for the player to accelerate from 0 to the runMaxSpeed.
    [HideInInspector] public float runAccelAmount; //The actual force (multiplied with speedDiff) applied to the player.
    public float runDecceleration; //Time (approx.) we want it to take for the player to accelerate from runMaxSpeed to 0.
    [HideInInspector] public float runDeccelAmount; //Actual force (multiplied with speedDiff) applied to the player .
    [Space(10)]
    [Range(0.01f, 1)] public float accelInAir; //Multipliers applied to acceleration rate when airborne.
    [Range(0.01f, 1)] public float deccelInAir;

    public float LastJumpTime;
    public bool isJumping, fall, grounded, isJumpCut;
    public float jumpHeight; //Height of the player's jump
    public float jumpTimeToApex;
    public float jumpForce;

    [Header("Gravity")]
    [HideInInspector] public float gravityStrength; //Downwards force (gravity) needed for the desired jumpHeight and jumpTimeToApex.
    [HideInInspector] public float gravityScale; //Strength of the player's gravity as a multiplier of gravity (set in ProjectSettings/Physics2D).
                                                 //Also the value the player's rigidbody2D.gravityScale is set to.
    [Space(5)]
    public float fallGravityMult; //Multiplier to the player's gravityScale when falling.
    public float maxFallSpeed; //Maximum fall speed (terminal velocity) of the player when falling.
    [Space(5)]
    public float jumpCutGravityMult;
    [Range(0f, 1)] public float jumpHangGravityMult; //Reduces gravity while close to the apex (desired max height) of the jump
    public float jumpHangTimeThreshold; //Speeds (close to 0) where the player will experience extra "jump hang". The player's velocity.y is closest to 0 at the jump's apex (think of the gradient of a parabola or quadratic function)
    [Space(0.5f)]
    public float jumpHangAccelerationMult;
    public float jumpHangMaxSpeedMult;
    [Header("Assists")]
    [Range(0.01f, 0.5f)] public float coyoteTime; //Grace period after falling off a platform, where you can still jump
    [Range(0.01f, 0.5f)] public float jumpInputBufferTime; //Grace period after pressing jump where a jump will be automatically performed once the requirements (eg. being grounded) are met.

    public bool doConserveMomentum, attacking;
    //public bool onBoat { get; private set; }
    public bool onBoat, sliding, crouching, onSteps, playerOnIce, onRamp, onIceRamp, invertedPlayer, playerStopped, gamePaused;
    public GameObject playerInverted;
    private float attackTimer, slideTimer;
    CircleCollider2D attackCollider;
    PlatformMovementHorizontal pmh;
    PlatformMovementVertical pmv;
    PlatformMovementDiagonal pmd;
    AudioSource source;
    public AudioClip swordSound, landingSound;
    public bool onSurfboard;
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        SetGravityScale(gravityScale);
        this.platform = null;
        circleCollider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        IsFacingRight = true;
        Scene scene = SceneManager.GetActiveScene();
        sceneName = scene.name;
        source = GetComponent<AudioSource>();
        GameObject ChildGameObject1 = this.transform.GetChild(1).gameObject;
        attackCollider = ChildGameObject1.GetComponent<CircleCollider2D>();
    }
    void Update()
    {
        if(!gamePaused)
        {
            LastOnGroundTime -= Time.deltaTime;
            LastJumpTime -= Time.deltaTime;

            if (!attacking || !sliding)
            {//If the player isnt attacking or sliding they can move
                _movement.x = Input.GetAxisRaw("Horizontal");
            }
            if (sliding)
            {
                source.mute = true;
                slideTimer += Time.deltaTime;
                if (playerOnIce)
                {
                    _movement.x = _movement.x / 1.003f;
                }
                else
                {
                    _movement.x = _movement.x / 1.01f;
                }
                //_movement.x = _movement.x / 1.02f;
                //Slow the player when sliding unless on ice
            }
            else
            {
                source.mute = false;
                animator.SetBool("Sliding", false);
            }
            if (playerOnIce)
            {
                if (slideTimer >= 1.6f)
                {//Stop sliding animation when stopped sliding
                    sliding = false;
                    animator.SetBool("Sliding", false);
                    slideTimer = 0;
                    animator.SetBool("Crouching", true);
                    crouching = true;
                }
            }
            else
            {
                if (slideTimer >= 0.9f)
                {//Stop sliding animation when stopped sliding
                    sliding = false;
                    animator.SetBool("Sliding", false);
                    slideTimer = 0;
                    animator.SetBool("Crouching", true);
                    crouching = true;
                }
            }
            if ((crouching && grounded))
            {//stop the player from moving when crouching
                if (!playerOnIce)
                {
                    _rigidbody.velocity = new Vector2(0, 0);
                }
                else
                {//When the player is on ice it will carry the momentum when sliding/crouching
                    _rigidbody.velocity = _rigidbody.velocity / 1.03f;
                }
                crouchCollider();
                if (_movement.x >= 0.1f)
                {
                    _movement.x = 0.1f;
                }
                else if (_movement.x <= -0.1f)
                {
                    _movement.x = -0.1f;
                }
            }
            else
            {
                //animator.SetBool("Crouching", false);
            }
            if (onRamp && !grounded)
            {
                _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            if (_movement.x != 0)
            {//Check if the player is moving
                if (!this.GetComponent<Player>().onVine)
                {
                    if (onRamp)
                    {
                        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
                    }
                    if (grounded)
                    {
                        if (!source.isPlaying && _rigidbody.velocity.x != 0)
                        {
                            source.Play();
                        }
                        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown("s"))
                        {//Detect when the down arrow key is pressed down
                            animator.SetBool("Sliding", true);
                            sliding = true;
                            animator.SetBool("Crouching", false);
                            crouching = false;
                            slidingCollider();
                        }
                        if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp("s"))
                        {//Detect when the down arrow key has been released
                            animator.SetBool("Sliding", false);
                            sliding = false;
                            animator.SetBool("Crouching", false);
                            crouching = false;
                            defaultCollider();
                        }
                    }
                    else
                    {
                        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown("s"))
                        {//Detect when the down arrow key is pressed down
                            animator.SetBool("Sliding", true);
                            sliding = true;
                            animator.SetBool("Crouching", false);
                            crouching = false;
                        }
                        //Detect when the down arrow key has been released
                        if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp("s"))
                        {
                            animator.SetBool("Sliding", false);
                            animator.SetBool("Crouching", false);
                            sliding = false;
                            crouching = false;
                            defaultCollider();
                        }
                    }
                }
                CheckDirectionToFace(_movement.x > 0);
                //Change the direction the player is facing when moving
            }
            else
            {
                if(!source.isPlaying)
                {
                    source.Stop();
                }
                
                if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown("s") && !this.GetComponent<Player>().onVine)
                {
                    if (sliding)
                    {
                        slidingCollider();
                    }
                    else
                    {
                        animator.SetBool("Sliding", false);
                        sliding = false;
                        crouching = true;
                        animator.SetBool("Crouching", true);
                        if (grounded)
                        {

                            crouchCollider();
                        }
                    }
                }
                //Detect when the down arrow key has been released to stop crouching
                if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp("s"))
                {
                    animator.SetBool("Crouching", false);
                    crouching = false;
                    defaultCollider();
                }
                if ((onRamp) && grounded)
                {
                    _rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
                }

            }
            if (platform != null)
            {//Check if player is on a platform
                transform.parent = platform;
                if (transform.position.y < platform.transform.position.y)
                {
                    transform.parent = null;
                }
            }
            else transform.parent = null;

            if (LastOnGroundTime > 0f && !isJumping)
            {//Check if player is on the ground
                isJumpCut = false;
                fall = false;
                //grounded = true;
                if (this.GetComponent<Player>().onVine)
                {
                    grounded = false;
                }
                else
                {
                    grounded = true;
                    jumpReturned = true;
                }
            }
            if (sceneName == "HadesRoom" || sceneName == "AresRoom")
            {
                if ((Input.GetButtonUp("Fire1") || Input.GetMouseButtonUp(0)) && grounded && !attacking)
                {//Plays player attack animation
                    playerAttack();
                }
            }
            if (attacking)
            {//Stop the player from moving when attacking and enables the hit collider and animation
                _rigidbody.velocity = new Vector2(0, 0);
                attackTimer += Time.deltaTime;
                if (attackTimer > 1.2f)
                {
                    attacking = false;
                    attackTimer = 0f;
                }
                else if (attackTimer > 1f)
                {
                    animator.SetBool("Attack", false);
                    attackCollider.enabled = false;
                }
                else if (attackTimer > 0.5f)
                {
                    attackCollider.enabled = true;
                }
            }

            if (!isJumping)
            {
                if ((Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) && !isJumping) || (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, platformLayer) && !isJumping) || (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, climbingLayer) && !isJumping))
                {//If the player has just left the ground they are still "grounded" for a time allowing smoother jumping at ledges
                 //LastOnGroundTime = 0.1f;
                    LastOnGroundTime = coyoteTime;
                }
                else
                {
                    //animator.SetBool("Falling", true);
                    //sliding = false;
                    //crouching = false;
                }
            }

            if (LastOnGroundTime < 0f)
            {//Check if the player is not on the ground
                grounded = false;
            }

            if (isJumping && _rigidbody.velocity.y < 0)
            {
                isJumping = false;
            }
            if (!isJumping && _rigidbody.velocity.y < 0 && !grounded)
            {//Dont let the player jump when falling         
                fall = true;
            }
            if (isJumpCut)
            {//If the player lets go of the jump button the jump will end early
                SetGravityScale(gravityScale * jumpCutGravityMult);
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, Mathf.Max(_rigidbody.velocity.y, -maxFallSpeed));
            }
            else if (isJumping && Mathf.Abs(_rigidbody.velocity.y) < jumpHangTimeThreshold)
            {
                SetGravityScale(gravityScale * jumpHangGravityMult);
            }
            else if (_rigidbody.velocity.y < 0 && !onRamp && !onIceRamp)
            {
                SetGravityScale(gravityScale * fallGravityMult);
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, Mathf.Max(_rigidbody.velocity.y, -maxFallSpeed));
            }
            else
            {
                SetGravityScale(gravityScale);
            }
        }
        else
        {
            
        }
        
    }
    void FixedUpdate()
    {
        Run();       
    }
    void LateUpdate()
    {
        if (CanJump() && LastJumpTime > 0)
        {//Checks if the player is jumping
            //Debug.Log(LastJumpTime);
            isJumping = true;
            isJumpCut = false;
            Jump();
        }
        if (Input.GetButtonDown("Jump") && !attacking)
        {//Lets the player jump when not attacking
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            onRamp = false;
            OnJump();
        }

        if (Input.GetButtonUp("Jump"))
        {
            OnJumpUpInput();
        }
        if (_movement.x != 0 && !onSurfboard)
        {//Player animation control
            animator.SetBool("Run", true);           
        }
        else
        {
            animator.SetBool("Run", false);
        }
        if(grounded)
        {
            animator.SetBool("Grounded", true);
        }
        else
        {
            animator.SetBool("Grounded", false);
            animator.SetBool("Falling", true);
        }
        if(isJumping)
        {
            animator.SetBool("Jump", true);
            animator.SetBool("OnLadder", false);
            animator.ResetTrigger("Climbing");
            animator.SetBool("Crouching", false);
        }
        else
        {
            animator.SetBool("Jump", false);
        }
        if(fall)
        {
            animator.SetBool("Falling", true);
        }
        else
        {
            animator.SetBool("Falling", false);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //If the object hit is a platform it will become the parent of the player
        if ((collision.gameObject.CompareTag("MPH") || collision.gameObject.CompareTag("MPV")) || collision.gameObject.CompareTag("MPD") && isOnThePlatform)
        {
            isOnThePlatform = true;
        }
        if (collision.gameObject.CompareTag("MPH") || collision.gameObject.CompareTag("MPV") || collision.gameObject.CompareTag("MPD") || collision.gameObject.CompareTag("MP"))
        {
            this.platform = collision.gameObject.transform;
            if (collision.gameObject.CompareTag("MPH"))
            {
                pmh = collision.gameObject.GetComponent<PlatformMovementHorizontal>();
                if (!pmh.enabled)
                {
                    pmh.enabled = true;
                    pmh.isTrigger = true;
                }
                if (pmh.isBoat)
                {
                    onBoat = true;
                }
            }
            if (collision.gameObject.CompareTag("MPV"))
            {
                pmv = collision.gameObject.GetComponent<PlatformMovementVertical>();
                if (!pmv.enabled)
                {
                    pmv.enabled = true;
                    pmv.isTrigger = true;
                }
            }
            if (collision.gameObject.CompareTag("MPD"))
            {
                pmd = collision.gameObject.GetComponent<PlatformMovementDiagonal>();
                if (!pmd.enabled)
                {
                    pmd.enabled = true;
                    pmd.isTrigger = true;
                }
                if (pmd.isBoat)
                {
                    onBoat = true;
                }
            }
            if (collision.gameObject.CompareTag("MP"))
            {
                pmv = collision.gameObject.GetComponent<PlatformMovementVertical>();
                if (pmv.stepPlatform)
                {
                    pmv.enabled = true;
                    pmv.isTrigger = true;
                    onSteps = true;
                }
            }
        }
        else if(collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("EnemyHit") || collision.gameObject.CompareTag("Bound"))
        {
            //Stops the player from losing a platform as its parent
        }
        else if (collision.gameObject.CompareTag("SSP"))
        {//Let the sprite rotate on different angled floors
            _rigidbody.constraints = RigidbodyConstraints2D.None;
            onIceRamp = true;
            //_rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        else if (collision.gameObject.CompareTag("Ramp"))
        {
            OnMud();
            Vector3 rt = new Vector3(0.0f, 0.0f, 27.0f);
            transform.rotation = Quaternion.Euler(rt);
            onRamp = true;
            //_rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            //_rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        else if (collision.gameObject.CompareTag("RampFlat"))
        {
            OnMud();
            Vector3 rt = new Vector3(0.0f, 0.0f, 0.0f);
            transform.rotation = Quaternion.Euler(rt);
            onRamp = true;
            //_rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            //_rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        else if (collision.gameObject.CompareTag("RampInv"))
        {
            OnMud();
            Vector3 rt = new Vector3(0.0f, 0.0f, -27.0f);
            transform.rotation = Quaternion.Euler(rt);
            onRamp = true;
            //_rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            //_rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        else if (collision.gameObject.CompareTag("Ice"))
        {//Make the ground more slippery when on ice
            _rigidbody.mass = 1.3f;
            runMaxSpeed = 14f;
            runAcceleration = 4f;
            runDecceleration = 0.9f;
            OnIce();
            playerOnIce = true;
            //doConserveMomentum = true;
        }
        else if (collision.gameObject.CompareTag("reset"))
        {//reset the rigidbody back to its original preset
            ResetRigidbody();
        }
        else
        {
            onRamp = false;
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            this.platform = null;
            if(onSteps && !pmv.reset)
            {
                if (pmv.stepPlatform && pmv.isTrigger)
                {
                    pmv.reset = true;
                    onSteps = false;
                }
            }
        }

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Enemy") && !collision.gameObject.CompareTag("EnemyHit") && !collision.gameObject.CompareTag("Bound"))
        {
            this.platform = null;
        }
        if (collision.gameObject.CompareTag("MPD"))
        {
            pmd.hasPlayer = false;
        }
        if (collision.gameObject.CompareTag("SSP"))
        {
            transform.rotation = Quaternion.Euler(Vector3.forward * 0f);
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            onIceRamp = false;
        }
        if (collision.gameObject.CompareTag("Ramp"))
        {
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            //ResetRigidbody();
            if (grounded == false)
            {
                onRamp = false;
            }
            //SetGravityScale(10f);
        }
        if (collision.gameObject.CompareTag("RampFlat"))
        {
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            //ResetRigidbody();
            if (grounded == false)
            {
                onRamp = false;
            }
            //SetGravityScale(10f);
        }
        if (collision.gameObject.CompareTag("RampInv"))
        {
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            //ResetRigidbody();
            if (grounded == false)
            {
                onRamp = false;
            }
        }
        if (collision.gameObject.CompareTag("Ice"))
        {//Return the players movement speed to normal when not on ice
            ResetRigidbody();
            OnIce();
            playerOnIce = false;
            //doConserveMomentum = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("SpeedBoost"))
        {
            if (IsFacingRight)
            {
                _rigidbody.AddForce(Vector2.right * 30, ForceMode2D.Impulse);
            }
            else
            {
                _rigidbody.AddForce(Vector2.left * 30, ForceMode2D.Impulse);
            }
        }
    }
    public void SetGravityScale(float scale)
    {
        _rigidbody.gravityScale = scale;
    }

    private void Run()
    {

        //Calculate the direction we want to move in and our desired velocity
        float targetSpeed = _movement.x * runMaxSpeed;
        float targetSpeed2 = _movement.x * runMaxSpeed * -1;
        #region Calculate AccelRate
        float accelRate;

        //Gets an acceleration value based on if we are accelerating (includes turning) 
        //or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
        if (LastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelAmount : runDeccelAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelAmount * accelInAir : runDeccelAmount * deccelInAir;

        #endregion

        //Not used since no jump implemented here, but may be useful if you plan to implement your own

        #region Add Bonus Jump Apex Acceleration
        //Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
        if ((isJumping) && Mathf.Abs(_rigidbody.velocity.y) < jumpHangTimeThreshold)
        {
            accelRate *= jumpHangAccelerationMult;
            targetSpeed *= jumpHangMaxSpeedMult;
            //targetSpeed2 *= jumpHangMaxSpeedMult;
        }
        #endregion


        #region Conserve Momentum
        //We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
        if (doConserveMomentum && Mathf.Abs(_rigidbody.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(_rigidbody.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
            //Prevent any deceleration from happening, or in other words conserve are current momentum
            //You could experiment with allowing for the player to slightly increase their speed whilst in this "state"
            accelRate = 50;
        }
        #endregion

        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - _rigidbody.velocity.x;
        float speedDif2 = targetSpeed2 - _rigidbody.velocity.x;
        //Calculate force along x-axis to apply to thr player

        float movement = speedDif * accelRate;
        float movement2 = speedDif2 * accelRate;
        //Convert this to a vector and apply to rigidbody
        if (!crouching)
        {
            if(!invertedPlayer)
            {
                _rigidbody.AddForce(movement * Vector2.right, ForceMode2D.Force);
            }
            else
            {//The players movement in aphrodite's room is reversed to appear mirrored
                _rigidbody.AddForce(movement2 * Vector2.right, ForceMode2D.Force);
            }

        }
        

        /*
		 * For those interested here is what AddForce() will do
		 * RB.velocity = new Vector2(RB.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / RB.mass, RB.velocity.y);
		 * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
		*/
    }

    private void Turn()
    {
        //stores scale and flips the player along the x axis, 
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        IsFacingRight = !IsFacingRight;
    }

    private void Jump()
    {//Add force to the player when jump is pressed
        this.GetComponent<Player>().playerClimb = false;
        source.Stop();
        jumpReturned = false;
        LastOnGroundTime = 0;
        LastJumpTime = 0;
        float force = jumpForce;
        if(this.GetComponent<Player>().onVine)
        {
            force += 3f;
            //this.platform = null;
        }
        else if(playerOnIce)
        {
            force += 3f;
        }
        if(_rigidbody.velocity.y < 0)
        {
            force -= _rigidbody.velocity.y;
        }

        _rigidbody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        this.GetComponent<Player>().jumpSoundPlay();           
    }
    public void OnJump()
    {
        LastJumpTime = jumpInputBufferTime;
        crouching = false;
        sliding = false;
        //onRamp = false;
        slideTimer = 0;     
    }
    public void OnJumpUpInput()
    {
        if (CanJumpCut())
            isJumpCut = true;
    }
    public void playerAttack()
    {
        attacking = true;
        animator.SetBool("Attack", true);
        Invoke("playerAttackSound", 0.3f);
    }
    public void playerAttackSound()
    {
        source.PlayOneShot(swordSound, 1f);
    }
    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
            Turn();
    }
    private bool CanJump()
    {
        return LastOnGroundTime > 0 && !isJumping && jumpReturned;
    }
    private bool CanJumpCut()
    {
        return isJumping && _rigidbody.velocity.y > 0;
    }
    private void OnValidate()
    {
        gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
        //Calculate the rigidbody's gravity scale (ie: gravity strength relative to unity's gravity value, see project settings/Physics2D)
        gravityScale = gravityStrength / Physics2D.gravity.y;
        jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;
        //Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
        runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
        runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;

        #region Variable Ranges
        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
        #endregion
    }
    public void OnIce()
    {//Change the players acceleration/decceleration when on ice to simulate the lack of friction
        runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
        runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;
        #region Variable Ranges
        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
        #endregion
    }
    public void OnMud()
    {//Lower players speed on certain terrain
        //_rigidbody.mass = 1.1f;
        runMaxSpeed = 12f;
    }
    public void ResetRigidbody()
    {
        _rigidbody.mass = 1f;
        runMaxSpeed = 13f;
        runAcceleration = 3f;
        runDecceleration = 6f;
    }
    public void crouchCollider()
    {//change the players collider when crouching
        this.GetComponent<BoxCollider2D>().offset = new Vector2(-0.03630257f, -0.6409789f);
        this.GetComponent<BoxCollider2D>().size = new Vector2(1.138664f, 1.352801f);
    }
    public void slidingCollider()
    {//change the players collider when sliding
        this.GetComponent<BoxCollider2D>().offset = new Vector2(0.1765658f, -0.7822303f);
        this.GetComponent<BoxCollider2D>().size = new Vector2(1.472023f, 1.070298f);
    }
    public void defaultCollider()
    {//set the players collider back to the default values
        this.GetComponent<BoxCollider2D>().offset = new Vector2(-0.03630257f, -0.2821921f);
        this.GetComponent<BoxCollider2D>().size = new Vector2(1.138664f, 2.070374f);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Draw feet
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
    }
}
