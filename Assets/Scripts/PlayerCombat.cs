using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    public Collider2D NlightHitbox;
    public LayerMask enemyLayers;

    private void OnTriggerEnter2D(Collider2D other)
    {
        int layer = other.gameObject.layer;

        if (((1 << layer) & enemyLayers) != 0)
        {
            Debug.Log("We hit " + other.gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Nlight();
        }
    }

    void Nlight()
    {
        animator.SetTrigger("Nlight");
    }
}
