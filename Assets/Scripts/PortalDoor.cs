using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalDoor : MonoBehaviour
{
    Animator myAnimator;
    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            myAnimator.SetBool("Opening", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            myAnimator.SetBool("Opening", false);
        }
    }
}
