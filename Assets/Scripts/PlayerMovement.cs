using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour {


    private Animator anime;


    [Header("Jumping Options")]
    private bool isJumping;
    private bool jump = false;
    public float jumpTime;
    private float jumpTimeCounter;
    public float jumpForce = 100f;
    public float extraJumpsValue = 1;
    private float extraJumps;
    public float fallMultiplier = 2.5f;
    public float lowKeyMultiplier = 1f;

    [Header("Movement Options")]

    public float sprintMultiplier = 1.3f;
    public GameObject walkParticle;
    public Transform wallCheck;
    public float wallCheckDistance;
    private RaycastHit2D wallCheckHit;
    public float slideSpeed;
    private bool canSlide;
    private bool allowedToSlide;

    private bool isSprinting;

    public Collider2D crouchDisable;
    public float crouchSpeed;
    private float crouchNormal;
    private bool isCrouching;

    [Header("Player Options")]
    public float health = 3;

    public float playerSpeed = 8f;
    public Rigidbody2D rb;

    private bool isGrounded = true;
    public Transform groundCheck;
    public Transform ceilingCheck;
    private bool hiCeiling;
    public float checkRadius;
    public LayerMask whatIsTile;
    public LayerMask whatIsWall;

    [Header("Attack Options")]
    public Transform attackPos;
    public LayerMask whatIsEnemy;
    public float attackRange;
    private float attackCDCounter = 0;
    public float attackCD;
    public int swordDamage;


    private float moveHorizontal;




    [Header("Friction Options")]
    public PhysicsMaterial2D noFriction;
    public PhysicsMaterial2D withFriction;

    [Header("Ability Options")]
    public bool abilitySlide = true;
    public bool abilityMinion = true;
    public bool abilityHealth = true;
    public bool abilityAttack = true;
    public float extraJumpLost = 1;

    void Start() {
        anime = GetComponent<Animator>();
        extraJumpsValue = extraJumpLost;
        extraJumps = extraJumpsValue;
        crouchNormal = playerSpeed;
    }

    void Update() {

        InputManagment();
        Ground();
        Slider();

    }

    void FixedUpdate() {
        
        Jump();
        Move();
        Flip();
        MinionSpawn();

    }



    #region Movement
    //private bool allowedToMove = true;
    private void Move() {
        rb.velocity = new Vector2(moveHorizontal, rb.velocity.y);

        if (wallCheckHit && !isGrounded) {
            //if (wallCheckHit.collider.tag == "WallJump") {
            extraJumps = extraJumpsValue;
            rb.velocity = new Vector2(rb.velocity.x, -1 * Time.fixedDeltaTime);
            //allowedToMove = false;
            canSlide = true;
           // }
        }
        else {
            rb.gravityScale = 4;
            canSlide = false;
        }
    }

    #region MovementDetails

    private void CeilingCheck() {
        hiCeiling = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsTile);
    }

    private void Slider() {
        if (abilitySlide) {
            Debug.DrawRay(transform.position, wallCheck.right * wallCheckDistance, Color.yellow);
            if (!isGrounded) {
                Debug.Log("HitTheWall");
                wallCheckHit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.2f), wallCheck.right, wallCheckDistance, whatIsWall);
            }
        }
    }

    private void Ground() {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsTile);
    }

    private void Flip() {

        if (moveHorizontal > 0) {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (moveHorizontal < 0) {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
    }


    public bool minionSpawns = false;
    public GameObject minionObject;
    public float minionCD;
    private float minionCDCounter = 0;

    public CanvasRenderer minionRenderer;




    void MinionSpawn() {
        if (GameObject.FindGameObjectWithTag("minion") == null) { 
        if (abilityMinion) {
            if (minionSpawns) {
                Debug.Log("spawnMinion");
                if (minionCDCounter <= 0) {
                    Debug.Log("cdMinion");
                    Instantiate(minionObject, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>().AddForce(new Vector2(transform.position.x + Random.Range(-1, 1), transform.position.y + Random.Range(-1, 1)));
                    minionCDCounter = minionCD;
                    
                }

            }
            if (minionCDCounter > 0) {
                //grayscale
                minionRenderer.SetColor(Color.black);
                minionCDCounter -= Time.deltaTime;
            }
            else {
                minionRenderer.SetColor(Color.white);
                //shiny
            }
        }
        else {
            //no more skill
        }
    }
    }

    void MinionSpawnInput() {
        if (Input.GetButtonDown("Fire2")) {
            minionSpawns = true;
        }
        else if (Input.GetButtonUp("Fire2")) {
            minionSpawns = false;
        }
    }

    void Jump() {

        if (rb.velocity.y < 0)
            anime.SetBool("Fall", true);

        if (rb.velocity.y < 0 && jump) {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        if (rb.velocity.y > 0 && !jump) {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowKeyMultiplier - 1) * Time.fixedDeltaTime;
        }


        if (jump && extraJumps > 0) {
            jumpTimeCounter = jumpTime;
            isJumping = true;
            rb.velocity = Vector2.up * jumpForce + Vector2.right * rb.velocity.x;
            extraJumps--;
        }

        if (Input.GetButton("Jump") && isJumping && jumpTimeCounter > 0 && !isGrounded) {
            rb.velocity += Vector2.up * jumpForce * Time.fixedDeltaTime;
            jumpTimeCounter -= Time.fixedDeltaTime;
        }

        if (isGrounded) {
            extraJumps = extraJumpsValue;
            jump = false;
            anime.SetBool("Jump", false);
            anime.SetBool("Fall", false);
            anime.SetBool("Ground", true);
        } else
            anime.SetBool("Ground", false);

    }

    #endregion

    #endregion

    #region Input
    void InputManagment() {



        SimpleMove();
        AttackInput();
        JumpInput();
        Sprint();
        WallJump();
        Crouch();
        MinionSpawnInput();



    }


    private void Crouch() {
        if (Input.GetButtonDown("Crouch")) {
            isCrouching = true;
            playerSpeed = crouchSpeed;
            crouchDisable.enabled = false;
        }
        else if (Input.GetButtonUp("Crouch")) {
            isCrouching = false;
            playerSpeed = crouchNormal;
            crouchDisable.enabled = true;
        }

    }

    private void SimpleMove() {
        moveHorizontal = Input.GetAxis("Horizontal") * playerSpeed;

        

        if (moveHorizontal != 0) {
            anime.SetFloat("Speed", 1);
            rb.sharedMaterial = noFriction;
            if (isSprinting && isGrounded) {

                Instantiate(walkParticle, new Vector2(transform.position.x, transform.position.y - .7f), Quaternion.identity);
            }
            //animator.SetBool("walking", true);
        }
        else {
            anime.SetFloat("Speed", 0);
            rb.sharedMaterial = withFriction;

            //animator.SetBool("walking", false);
        }
    }

    public Transform jumpCheck;
    private RaycastHit2D jumpe;
    public float jumpCheckDistance;


    private void WallJump() {

        if (canSlide) {



            if (Input.GetButtonDown("Jump")) {

                 jumpe = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.2f), -jumpCheck.right + jumpCheck.up, jumpCheckDistance, whatIsWall);
                 //YOU WILL TELEPORT REMEMBER HIT MIT BABE
                 Debug.DrawRay(transform.position, (-jumpCheck.right + jumpCheck.up) * jumpCheckDistance, Color.green);
                 /*if (jumpe) {
                     if (transform.localRotation.y == -1)
                         transform.localRotation = Quaternion.Euler(0, 1, 0);
                     else
                         transform.localRotation = Quaternion.Euler(0, -1, 0);

                    transform.position = jumpe.point;
                }*/
                canSlide = false;
            }


        }
    }


    private void Sprint() {
        if (Input.GetButtonDown("Sprint") && isGrounded && !isSprinting) {
            isSprinting = true;
            playerSpeed *= sprintMultiplier;
        }
        if (Input.GetButtonUp("Sprint") && isSprinting) {
            isSprinting = false;
            playerSpeed /= sprintMultiplier;
        }

    }

    private void JumpInput() {
        if (Input.GetButtonDown("Jump")) {
            anime.SetBool("Jump", true);
            jump = true;
        }
        if (Input.GetButtonUp("Jump")) {
            isJumping = false;
        }

    }

    private void AttackInput() {
        if (abilityAttack) {

            if (attackCDCounter <= 0) {
                if (Input.GetButton("Fire1")) {
                    anime.Play("Attack");
                    Collider2D[] damageEnemies = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemy);

                    for (int i = 0; i < damageEnemies.Length; i++)
                        if (damageEnemies[i].GetType().ToString() == "UnityEngine.CapsuleCollider2D")
                            damageEnemies[i].GetComponent<Enemy>().TakeDamage(swordDamage, transform.localRotation.y);

                    attackCDCounter = attackCD;
                }
            }
            else
                attackCDCounter -= Time.deltaTime;
        }
    }


    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        Gizmos.DrawWireSphere(ceilingCheck.position, checkRadius);

    }
    #endregion

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "enemy") {
            health -= 1;
        }
        if (collision.tag == "portal") {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        if (collision.tag == "lava") {
            int i = 0;
            while (i != 10) {
                Instantiate(walkParticle, new Vector2(transform.position.x - Random.Range(-1,1), transform.position.y - Random.Range(-1, 1)), Quaternion.identity);
                i++;
                }
            StartCoroutine(killHim());
        }
    }

    IEnumerator killHim() {
        yield return new WaitForSeconds(.2f);
        killMe();
    }

    private void killMe() {
        Destroy(gameObject);
    }
}
