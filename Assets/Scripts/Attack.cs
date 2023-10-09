using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    Collider2D nlightCollider;

    public float NlightDmg = 10;
    private Vector2 baseForce = Vector2.zero;
    public float forceX = 10f;
    public float forceY = 50f;


    private void Awake()
    {
        nlightCollider = GetComponent<Collider2D>();
    }
    void Start()
    {
        
    }


    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();
        if (damageable != null)
        {
            Vector2 baseForce = new Vector2(forceX, forceY);
            damageable.Hit(NlightDmg, baseForce);
            Debug.Log(collision.name + " hit for " + NlightDmg);
        }
    }


}
