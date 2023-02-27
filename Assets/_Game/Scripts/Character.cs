using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private float hp;
    [SerializeField] protected HealthBar healthBar;
    [SerializeField] protected CombatText CombatTextPrefab;
    public bool IsDeath;
    protected string currentAnimName = "idle";

    private void Start() 
    {
        OnInit();   

    }
    public virtual void OnInit()
    {
        hp = 100;
        healthBar.OnInit(100, transform);
    }
    
    public virtual void OnDespawn()
    {
        gameObject.SetActive(false);
        Invoke(nameof(OnInit), 1f);
    }
    protected virtual void OnDeath()
    {
        ChangeAnim("die");

        Invoke(nameof(OnDespawn), 1f);
    }

    protected void ChangeAnim(string animName)
    {
        if(currentAnimName != animName)
        {
            anim.ResetTrigger(currentAnimName);
            currentAnimName = animName;
            anim.SetTrigger(currentAnimName);
        }
    }

    public void OnHit(float damege)
    {
        hp -= damege;
        if (hp <= 0) {
            hp = 0;
            IsDeath = true;
            OnDeath();
        }

        healthBar.SetNewHp(hp);
        Instantiate(CombatTextPrefab, transform.position + Vector3.up, Quaternion.identity).OnInit(damege);
    }

}
