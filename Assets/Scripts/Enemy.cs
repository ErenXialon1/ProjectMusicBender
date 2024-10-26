using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int enemyHp;
    public List<GameObject> enemyAttacks = new List<GameObject>();
    public float attackShotSpeed;
    public float attackCooldown = 2f;
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
     
        rb.linearVelocity = Vector2.left * attackShotSpeed;
        
    }
    public GameObject SelectedAttack()
    {
        int randomIndex = Random.Range(0, enemyAttacks.Count);
        return enemyAttacks[randomIndex];
        
    }
    
    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        AttackCreate();
       
        StartCoroutine(AttackCooldown());
        


    }
}
