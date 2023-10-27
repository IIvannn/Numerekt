using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public UnityEvent<float, Vector2> damageableHit;

    public Animator animator;

    [SerializeField] private float _maxHealth = 0;
    [SerializeField] private bool _isAlive = true;
    [SerializeField] private bool isInvincible = false;

    public bool IsHit
    {
        get { return animator.GetBool("isHit"); }
        private set { animator.SetBool("isHit", value); }
    }

    private float timeSinceHit = 0;
    public float invincibilityTime = 0.25f;

    public float MaxHealth
    {
        get { return _maxHealth; }
        set
        {
            _maxHealth = value;
            if (_health < 0)
            {
                isAlive = false;
            }
        }
    }

    [SerializeField] private float _health = 0;

    public float Health
    {
        get { return _health; }
        set { _health = value; }
    }

    public bool isAlive
    {
        get { return _isAlive; }
        set
        {
            _isAlive = value;
            Debug.Log("isAlive set " + value);
        }
    }

    public bool isStunned = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isStunned)
        {
            // Placeholder logic for the stun effect
            Debug.Log("Character is stunned!");
        }

        if (isInvincible)
        {
            if (timeSinceHit > invincibilityTime)
            {
                isInvincible = false;
                timeSinceHit = 0;
            }
            timeSinceHit += Time.deltaTime;
        }
    }

    public void Hit(float damage, Vector2 totalForce)
    {
        if (isAlive && !isInvincible)
        {
            Health += damage;
            isInvincible = true;
            IsHit = true;

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.velocity = totalForce;

            damageableHit?.Invoke(damage, totalForce);

            if (Health <= 0)
            {
                isAlive = false;
                Health = 0;
            }
        }
    }

    private Coroutine stunCoroutine; 

    public void StunCharacter(float durationInSeconds)
    {
        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine); 
        }

        isStunned = true;
        stunCoroutine = StartCoroutine(DisableStunAfterDuration(durationInSeconds));
    }

    private IEnumerator DisableStunAfterDuration(float durationInSeconds)
    {
        yield return new WaitForSeconds(durationInSeconds);
        isStunned = false;
    }
}
