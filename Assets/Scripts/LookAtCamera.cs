using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = FindObjectOfType<CameraMove>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + mainCamera.forward); 
    }
}
