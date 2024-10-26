

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;



public class UpdatePlayerAttacks : MonoBehaviour
{
    public List<GameObject> spriteBlocks = new List<GameObject>();
    public List<Sprite> sprites = new List<Sprite>();
    public Player playerScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerScript = GameObject.Find("Player").GetComponent<Player>();
        foreach (GameObject sprite in spriteBlocks)
        {
            {
                sprites.Add(sprite.GetComponent<Image>().sprite);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GetInputtedAttacks()
    {

    }
}
