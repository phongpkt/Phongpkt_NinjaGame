using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float speed = 1000;
    [SerializeField] private float jumpForce = 500;

    [SerializeField] private bool isGrounded = true;
    [SerializeField] private bool isJumping = false;
    [SerializeField] private bool isAttack = false;

    [SerializeField] private float horizontal;

    [SerializeField] private int coin = 0;

    [SerializeField] private Kunai kunaiPrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private GameObject attackArea;
    private Vector3 savePoint;

private void Awake() 
{
    coin = PlayerPrefs.GetInt("coint", 0);
}
    //protected string currentAnimName;

    private void Update() 
    {
        if (isAttack)
        {
            //rb.velocity = Vector2.zero;
            return;
        }
        //horizontal = Input.GetAxis("Horizontal");
        //transform.position = savePoint;
        // ChangeAnim("idle");
        DeactiveAttack();

        isGrounded = CheckGrounded();

        //-1 -> 0 -> 1
        //horizontal = Input.GetAxisRaw("Horizontal");
        //verticle = Input.GetAxisRaw("Vertical");

        if (isGrounded)
        {
            if (isJumping)
            {
                return;
            }

            //jump
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                Jump();
                return;
            }
 
            //change anim run
            if (Mathf.Abs(horizontal) > 0.1f)
            {
                ChangeAnim("run");
            }

            //attack
            if (Input.GetKeyDown(KeyCode.C) && isGrounded)
            {
                rb.velocity = Vector2.zero;
                Attack();
                return;
            }

            //throw
            if (Input.GetKeyDown(KeyCode.V) && isGrounded)
            {
                Throw();
                return;
            }
        }

        //check falling
        if (!isGrounded && rb.velocity.y < 0)
        {
            ChangeAnim("fall");
            isJumping = false;
        }
    }

    public override void OnInit()
    {
        base.OnInit();
        IsDeath = false;
        isAttack = false;
        //SavePoint();
        rb.velocity = Vector2.zero;
        transform.position = savePoint;
        gameObject.SetActive(true);

        UIManager.instance.SetCoin(coin);
        
    }

    // protected override void OnDeath()
    // {
    //     rb.velocity = Vector2.zero;
    //     base.OnDeath();
    // }

    private void FixedUpdate() {
        if (isAttack || IsDeath) return;

        //Moving
        if (Mathf.Abs(horizontal) > 0.1f)
        {
            rb.velocity = new Vector2(horizontal * speed * Time.fixedDeltaTime, rb.velocity.y);
            //horizontal > 0 -> tra ve 0, neu horizontal <= 0 -> tra ve la 180
            transform.rotation = Quaternion.Euler(new Vector3(0, horizontal > 0 ? 0 : 180, 0));
            //transform.localScale = new Vector3(horizontal, 1, 1);
        }
        //idle
        else if (isGrounded && !isJumping && !isAttack)
        {
            ChangeAnim("idle");
            rb.velocity = Vector2.up * rb.velocity.y;
        }
    }

    private bool CheckGrounded()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.down * 1.1f, Color.red );
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);

        return hit.collider != null;
    }

    public void Attack()
    {
        if(isAttack == true)
        {
            return;
        }
        isAttack = true;
        //rb.velocity = Vector2.zero;
        ChangeAnim("attack");
        Invoke(nameof(ResetAttack), 0.5f);
        ActiveAttack();
        Invoke(nameof(DeactiveAttack), 0.5f);
    }

    public void Throw()
    {
        if(isAttack == true)
        {
            return;
        }
        ChangeAnim("throw");
        isAttack = true;
        Invoke(nameof(ResetAttack), 1f);

        Instantiate(kunaiPrefab, throwPoint.position, throwPoint.rotation);
    }

    private void ResetAttack()
    {
        
        isAttack = false;
        ChangeAnim("idle");
    }

    public void Jump()
    {
        if(isGrounded == false)
        {
            return;
        }
        isGrounded = false;
        isJumping = true;
        ChangeAnim("jump");
        rb.AddForce(jumpForce * Vector2.up);
    }


    internal void SavePoint()
    {
        savePoint = transform.position;
    }

    private void ActiveAttack()
    {
        attackArea.SetActive(true);
    }

    private void DeactiveAttack()
    {
        attackArea.SetActive(false);
    }

    public void SetMove(float horizontal)
    {
        this.horizontal = horizontal;
    }

    private void OnTriggerEnter2D(Collider2D collison) 
    {
        if(collison.tag == "Coin")
        {
            coin ++;
            PlayerPrefs.SetInt("coin", coin);
            UIManager.instance.SetCoin(coin);
            Destroy(collison.gameObject);
        }    
        if(collison.tag == "DeathZone")
        {
            IsDeath = true;
            ChangeAnim("die");
            Invoke(nameof(OnInit), 1f);
        }
    }
    
}
