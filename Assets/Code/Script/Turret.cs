using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Turret : MonoBehaviour {

    [Header("References")]
    [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private GameObject wizardObject; // Reference to the child object with animations
    private Animator wizardAnimator;

    [Header("Attributes")]
    [SerializeField] private float targetingRange = 5f;
    //[SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float bps = 1f; // bullets per second

    private Transform target;
    private float timeUntilFire;

    private void Start()
    {
        // If wizardObject is not assigned in inspector, try to find it
        if (wizardObject == null)
        {
            // Try to find a child object with an Animator component
            wizardAnimator = GetComponentInChildren<Animator>();
            if (wizardAnimator != null)
            {
                wizardObject = wizardAnimator.gameObject;
            }
            else
            {
                Debug.LogError("No child object with Animator found! Please assign the wizard object in the inspector.");
            }
        }
        else
        {
            // Get the animator from the assigned wizard object
            wizardAnimator = wizardObject.GetComponent<Animator>();
            if (wizardAnimator == null)
            {
                Debug.LogError("Assigned wizard object does not have an Animator component!");
            }
        }
    }

    private void Update()
    {
        if (target == null)
        {
            FindTarget();
            if (wizardAnimator != null)
            {
                wizardAnimator.SetBool("isAttacking", false);
            }
            return;
        }

        if (!CheckTargetIsInRange())
        {
            target = null;
            if (wizardAnimator != null)
            {
                wizardAnimator.SetBool("isAttacking", false);
            }
        }
        else
        {
            timeUntilFire += Time.deltaTime;

            if (timeUntilFire >= 1f / bps) {
                if (wizardAnimator != null)
                {
                    wizardAnimator.SetBool("isAttacking", true);
                }
                timeUntilFire = 0f;
            }
        }
    }
    
    // This needs to be public so it can be called from Animation Events
    public void Shoot() {
        if (target != null && CheckTargetIsInRange()) {
            GameObject bulletObj = Instantiate(bulletPrefab, firingPoint.position, Quaternion.identity);
            Bullet bulletScript = bulletObj.GetComponent<Bullet>();
            bulletScript.SetTarget(target);
        }
    }

    private void FindTarget()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, (Vector2)
        transform.position, 0f, enemyMask);

        if (hits.Length > 0)
        {
            target = hits[0].transform;
        }
    }

    public bool CheckTargetIsInRange() {
        if (target == null) return false;
        return Vector2.Distance(target.position, transform.position) <= targetingRange;
    }

    // If you need to check range for any position, you can use this overload
    public bool CheckPositionIsInRange(Vector2 position) {
        return Vector2.Distance(position, transform.position) <= targetingRange;
    }
    
    // private void RotateTowardsTarget() {
    //     float angle = Mathf.Atan2(target.position.y - transform.position.y, target.position.x -
    //     transform.position.x) * Mathf.Rad2Deg - 90f;

    //     Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
    //     turretRotationPoint.rotation = Quaternion.RotateTowards(turretRotationPoint.rotation,
    //     targetRotation, rotationSpeed * Time.deltaTime);
    // }

    // private void OnDrawGizmosSelected() {
    //     Handles.color = Color.cyan;
    //     Handles.DrawWireDisc(transform.position, transform.forward, targetingRange);
    // }

}
