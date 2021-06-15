using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed, bulletLife;

    public Rigidbody myRigidBody;

    public ParticleSystem explosionEffect;
    public bool isRocket;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //velocity represents the rate of change of Rigidbody position
        BulletFly();

        bulletLife -= Time.deltaTime;
        
        if (bulletLife < 0)
        {
            Destroy(gameObject);
        }
    }

    private void BulletFly()
    {
        myRigidBody.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isRocket)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
        //gameObject means the one that has the script attached
        Destroy(gameObject);
    }
}
