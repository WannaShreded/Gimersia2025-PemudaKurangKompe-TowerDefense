using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Attributes")]
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private int bulletDamage = 1;
    [SerializeField] private float bulletDestroyTime = 5f;

    private Transform target;
    private float timeAlive;

    public void SetTarget(Transform _target)
    {
        target = _target;
        timeAlive = 0f;
    }

    private void Update()
    {
        timeAlive += Time.deltaTime;
        if (timeAlive >= bulletDestroyTime)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void FixedUpdate()
    {
        if (!target) return;

        Vector2 direction = (target.position - transform.position).normalized;

        rb.velocity = direction * bulletSpeed;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        other.gameObject.GetComponent<Health>().TakeDamage(bulletDamage);
        Destroy(gameObject);
    }
}
