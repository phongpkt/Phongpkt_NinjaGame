using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] private float attackRange;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject attackArea;
    [SerializeField] private bool isAttack;
    private IState currentState;
    private bool isRight = true;
    private Character target;
    public Character Target => target;
    public bool isMoving = false;

    private void Update() 
    {
        if(currentState != null && !IsDeath)
        {
            currentState.OnExecute(this);
        }    
    }

    public override void OnInit()
    {
        base.OnInit();
        ChangeState(new IdleState());
        //DeactiveAttack();
    }
    public override void OnDespawn()
    {
        base.OnDespawn();
        Destroy(healthBar.gameObject);
        Destroy(gameObject);
    }

    protected override void OnDeath()
    {
        ChangeState(null);
        base.OnDeath();
    }

    public void ChangeState(IState newState)
    {
        if(currentState != null)
        {
            currentState.OnExit(this);
        }

        currentState = newState;

        if(currentState != null)
        {
            currentState.OnEnter(this);
        }
    }

    private void FixedUpdate() {
        if(IsDeath == true)
        {
            return;
        }
        if(isMoving){
            ChangeAnim("run");
            rb.velocity = transform.right * moveSpeed;
        }
        else if ( isAttack == false )
        {
            ChangeAnim("idle");
            rb.velocity = Vector2.zero;
        }
    }

    public void Attack()
    {   
        rb.velocity = Vector2.zero;
        ChangeAnim("attack");
        //Debug.Log("AimAttack");
        ActiveAttack();
        Invoke(nameof(DeactiveAttack), 0.5f);
    }

    public bool IsTargetInRange()
    {
        if(target != null && Vector2.Distance(target.transform.position, transform.position) <= attackRange)
        {
            return  true;
        }
        else
        {
            return false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if(collision.tag == "EnemyWall")
        {
            ChangeDirection(!isRight);
        }   
    }

    public void ChangeDirection(bool isRight)
    {
        this.isRight = isRight;

        transform.rotation = isRight ? Quaternion.Euler(Vector3.zero) : Quaternion.Euler(Vector3.up * 180);
    }

    internal void SetTarget(Character character)
    {
        this.target = character;
        if(Target != null)
        {
            ChangeState(new PatrolState());
        }
        else
        {
            ChangeState(new IdleState());
        }
        // if(IsTargetInRange())
        // {
        //     ChangeState(new AttackState());
        // }
        // else
        // {
        //     Moving();
        // }
    }
    private void ActiveAttack()
    {
        isAttack = true;
        attackArea.SetActive(true);
    }

    private void DeactiveAttack()
    {
        isAttack = false;
        attackArea.SetActive(false);
    }
}
