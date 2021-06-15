using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
    //get a reference to the nav mesh agent
    NavMeshAgent myAgent;
    public LayerMask whatIsGround, whatIsPlayer;

    public Transform player;
    Animator myAnimator;

    public Transform firePosition;

    //Guarding
    //create a destination for the agent
    public Vector3 destionationPoint;
    bool destinationSet;
    public float destinationRange;

    //Chasing
    public float chaseRange;
    private bool playerInChaseRange;

    //Attacking
    public float attackRange, attackTime;
    private bool playerInAttackRange, readyToAttack = true;
    public GameObject attackProjectile;

    //Melle
    public bool meleeAtacker;
    public int meleeDamageAmount;

    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        myAgent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<Player>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        playerInChaseRange = Physics.CheckSphere(transform.position, chaseRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInChaseRange && !playerInAttackRange) Guarding();
        if (playerInChaseRange && !playerInChaseRange) ChasingPlayer();
        if (playerInAttackRange && playerInChaseRange) AttackingPlayer();
    }

    private void Guarding()
    {
        //searching for a certain destination point
        if (!destinationSet)
        {
            SearchForDestination();
        }
        else
        {
            myAgent.SetDestination(destionationPoint);
        }

        Vector3 distanceToDestionation = transform.position - destionationPoint;

        if (distanceToDestionation.magnitude < 1f)
        {
            destinationSet = false;
        }
    }
    private void ChasingPlayer()
    {
        myAgent.SetDestination(player.position);
    }

    private void AttackingPlayer()
    {
        myAgent.SetDestination(transform.position);
        transform.LookAt(player);

        if (readyToAttack && !meleeAtacker)
        {
            myAnimator.SetTrigger("Attack");
            firePosition.LookAt(player);
            Instantiate(attackProjectile, firePosition.position, firePosition.rotation);

            readyToAttack = false;
            StartCoroutine(ResetAttack());
        } 
        else if (readyToAttack && meleeAtacker)
        {
            myAnimator.SetTrigger("Attack");
        }
    }

    private void SearchForDestination()
    {
        //create a random point for our agent to walk to
        float randPositionZ = Random.Range(-destinationRange, destinationRange);
        float randPositionX = Random.Range(-destinationRange, destinationRange);

        destionationPoint = new Vector3(
            transform.position.x + randPositionX,
            transform.position.y,
            transform.position.z + randPositionZ);

        if (Physics.Raycast(destionationPoint, - transform.up, 2f, whatIsGround))
        {
            destinationSet = true;
        }
    }

    public void MeleeDamage()
    {
        if (playerInAttackRange)
        {
            //demage the player
            player.GetComponent<PlayerHealthSystem>().TakeDamage(meleeDamageAmount);
        }
    }

    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(attackTime);
        readyToAttack = true;
    }

    //allows us to draw something in the gizmos
    //draw something around the chase range
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
