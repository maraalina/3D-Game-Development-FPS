using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileController : MonoBehaviour
{
    //add couple of forces to our bullet to make it jump to our player
    Rigidbody myRigidBody;
    public float upForce, forwardForce;

    public int damageAmount = 3;
    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();

        GrenadeThrow();
    }

    private void GrenadeThrow()
    {
        myRigidBody.AddForce(transform.forward * forwardForce, ForceMode.Impulse);
        myRigidBody.AddForce(transform.up * upForce, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerHealthSystem>().TakeDamage(damageAmount);
            Destroy(gameObject);
        }
    }
}
