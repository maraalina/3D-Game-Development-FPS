using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform myPlayerHead;

    private float startFOV, targetFOV;
    public float FOVSpeed = 1f;
    private Camera myCamera;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        myCamera = GetComponent<Camera>();
        startFOV = myCamera.fieldOfView;
        targetFOV = startFOV;
    }

    //called every frame after all the updates have been called
    //when our player finishes his movement, the camera will move to the position of the head on the player
    private void LateUpdate()
    {
        //move the camera position to player head position
        transform.position = myPlayerHead.position;
        //rotate the camera rotation with player head rotation
        transform.rotation = myPlayerHead.rotation;

        myCamera.fieldOfView = Mathf.Lerp(myCamera.fieldOfView, targetFOV, FOVSpeed * Time.deltaTime);
    }

    public void ZoomIn(float targetZoom)
    {
        targetFOV = targetZoom;
    }

    public void ZoomOut()
    {
        targetFOV = startFOV;
    }
}
