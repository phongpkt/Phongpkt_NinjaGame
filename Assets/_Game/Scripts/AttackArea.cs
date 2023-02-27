using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    bool isAttack = false;
    private void OnEnable() {
        isAttack = false;
    }
    private void OnTriggerStay2D(Collider2D collision) 
    {
        if(!isAttack && (collision.tag == "Player" || collision.tag == "Enemy"))
        {
            isAttack = true;
            collision.GetComponent<Character>().OnHit(20f);
        }
    }
}
