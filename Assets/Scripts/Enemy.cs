using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int enemyHp;
    public GameObject[] attacks = new GameObject[0];
    public float attackShotSpeed;
    void Start()
    {
        StartCoroutine(AttackCooldown());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AttackCreate()
    {
       GameObject selectedAttack = SelectedAttack();
       GameObject instantiatedAttack = Instantiate(selectedAttack, transform.position, selectedAttack.transform.rotation);
       AttackMove(instantiatedAttack);
    }
    public void AttackMove(GameObject _instantiatedAttack)
    {
        
        Rigidbody2D rb = _instantiatedAttack.GetComponent<Rigidbody2D>();
     
        rb.velocity = Vector2.left * attackShotSpeed;
        
    }
    public GameObject SelectedAttack()
    {
        int randomIndex = Random.Range(0, attacks.Length);
        return attacks[randomIndex];
        
    }
    
    IEnumerator AttackCooldown()
    {
        
        AttackCreate();
        yield return new WaitForSeconds(2f);
        StartCoroutine(AttackCooldown());
        


    }
}
