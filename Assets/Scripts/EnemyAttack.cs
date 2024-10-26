using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack :MonoBehaviour
{
    public int attackDamage = 25;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerAttack"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {

            DamageThePlayer();
        }
    }
    public void DamageThePlayer()
    {
        Player playerScript = GameObject.Find("Player").GetComponent<Player>();
        playerScript.playerHp -= attackDamage;
        if(playerScript.playerHp <= 0)
        {
            Destroy(playerScript.gameObject);
        }
    }
}
