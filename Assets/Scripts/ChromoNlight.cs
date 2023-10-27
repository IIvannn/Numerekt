using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChromoNlight : MonoBehaviour
{
    Collider2D nlightCollider;

    public float NlightDmg = 10;
    private Vector2 baseForce = Vector2.zero;
    public float forceX = 10f;
    public float forceY = 10f;
    public float variableForceX = 0.1f;
    public float variableForceY = 0.1f;
  
    
   

    public float stunDuration = 1.0f; 

    private void Awake()
    {
        nlightCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
{
    Damageable damageable = collision.GetComponent<Damageable>();
    if (damageable != null)
    {
        // Determine the direction the character is facing
        int direction = (transform.parent.localScale.x > 0) ? 1 : -1;

        // Calculate the force direction based on the character's direction and the current health of the damageable object
        float modifiedForceX = forceX * (1 + (damageable.MaxHealth - damageable.Health) * -variableForceX) * direction;
        float modifiedForceY = forceY * (1 + (damageable.MaxHealth - damageable.Health) * -variableForceY);

            // Apply the forces to the damageable object
            Vector2 totalForce = new Vector2(modifiedForceX, modifiedForceY);
        damageable.Hit(NlightDmg, totalForce);
        damageable.StunCharacter((float)(stunDuration * 0.0166666666666666)); // Apply the stun effect with the specified duration

            // Print the forces applied for debugging
            Debug.Log(collision.name + " hit for " + NlightDmg + " with modified force X: " + modifiedForceX + ", Y: " + modifiedForceY);
    }
}



}
