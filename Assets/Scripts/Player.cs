using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 12.5f, runSpeed = 25f;

    //create a referance in the code to keep track of the velocity of our player
    //adding Gravity
    public Vector3 velocity;
    public float gravityModifier;

    public CharacterController myCharacterController;
    public Transform myCameraHead;
    public Animator myAnimator;

    public float mouseSensitivity = 100f;
    private float cameraVerticalRotation;


    //jumping
    public float jumpHeight = 2f;
    private bool readyToJump;
    public Transform ground;
    public LayerMask groundLayer;
    public float groundDistance = 0.5f;

    //crouching
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 bodyScale;
    public Transform myBody;
    private float initialControllerHeight;
    public float crouchingSpeed = 6f;
    private bool isCrouching = false;

    //sliding
    private bool isRunning = false, startSlideTimer;
    private float currentSlideTimer, maxSlideTime = 2f;
    public float slideSpeed = 30f;

    //hook shot
    public Transform hitPointTransform;
    //check the position of the hookshot
    private Vector3 hookShotPosition;
    public float hookShotSpeed = 5f;
    private Vector3 flyingCharacterMomentum;
    public Transform grapplingHook;
    private float hookShotSize;
    public ParticleSystem warpParticles;

    //player states
    private State state;
    private enum State { Normal, HookShotFlyingPlayer, HookShotThrown }

    // Start is called before the first frame update
    void Start()
    {
        //initial player scale
        bodyScale = myBody.localScale;
        initialControllerHeight = myCharacterController.height;

        state = State.Normal;

        //set grappling hook off
        grapplingHook.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case State.Normal:
                PlayerMovement();
                CameraMovement();
                Jump();
                Crouching();
                HandleLookShotStart();
                SlideCounter();
                break;
            case State.HookShotThrown:
                PlayerMovement();
                CameraMovement();
                ThrowHook();
                break;
            case State.HookShotFlyingPlayer:
                CameraMovement();
                HandleHookShotMovement();
                break;

            default:
                break;
        }
    }

    private void Crouching()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCrouching();
        }

        if (Input.GetKeyUp(KeyCode.C) || currentSlideTimer > maxSlideTime)
        {
            StopCrouching();
        }
    }

    private void StartCrouching()
    {
        myBody.localScale = crouchScale;
        myCameraHead.position -= new Vector3(0, 1f, 0);
        myCharacterController.height /= 2;
        isCrouching = true;

        if (isRunning)
        {
            //returns the location of the vector on the plane
            velocity = Vector3.ProjectOnPlane(myCameraHead.transform.forward, Vector3.up).normalized * slideSpeed * Time.deltaTime;
            startSlideTimer = true;
        }
    }

    private void StopCrouching()
    {
        currentSlideTimer = 0f;
        velocity = new Vector3(0f, 0f, 0f);
        startSlideTimer = false;

        myBody.localScale = bodyScale;
        myCameraHead.position += new Vector3(0, 1f, 0);
        myCharacterController.height = initialControllerHeight;
        isCrouching = false;
    }

    //getting input from the jump button and adding velocity in the y direction
    void Jump()
    {
        readyToJump = Physics.OverlapSphere(ground.position, groundDistance, groundLayer).Length > 0;
        if (Input.GetButtonDown("Jump") && readyToJump)
        {
            AudioManager.instance.PlayerSFX(3);
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y) * Time.deltaTime;
        }
        //update the velocity
        myCharacterController.Move(velocity);
    }


    private void CameraMovement()
    {
        //frame rate independent
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        cameraVerticalRotation -= mouseY;
        //fixes a value between a min and max
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);
        //aplying the rotation to the body transform
        transform.Rotate(Vector3.up * mouseX);
        myCameraHead.localRotation = Quaternion.Euler(cameraVerticalRotation, 0f, 0f);
    }

    void PlayerMovement()
    {
        //move our character
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 movement = x * transform.right + z * transform.forward;

        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
        {
            movement = movement * runSpeed * Time.deltaTime;
            isRunning = true;
        }
        //detach velocity of our player from the number of frames
        else if (isCrouching)
        {
            movement = movement * crouchingSpeed * Time.deltaTime;
        }
        else
        {
            movement = movement * speed * Time.deltaTime;
            isRunning = false;
        }

        myAnimator.SetFloat("PlayerSpeed", movement.magnitude);

        movement += flyingCharacterMomentum * Time.deltaTime;

        Debug.Log(movement.magnitude);
        myCharacterController.Move(movement);

        //gravity formula
        velocity.y += Physics.gravity.y * Mathf.Pow(Time.deltaTime, 2) * gravityModifier;

        //reset the gravity velocity if the player is grounded
        if (myCharacterController.isGrounded)
        {
            velocity.y = Physics.gravity.y * Time.deltaTime;
        }
        //the player is affected by gravity
        myCharacterController.Move(velocity);

        if (flyingCharacterMomentum.magnitude > 0f)
        {
            float reductionAmount = 4f;
            flyingCharacterMomentum -= flyingCharacterMomentum * reductionAmount * Time.deltaTime;

            if (flyingCharacterMomentum.magnitude < 5f)
            {
                flyingCharacterMomentum = Vector3.zero;
            }
        }
    }

    private void SlideCounter()
    {
        if (startSlideTimer)
        {
            currentSlideTimer += Time.deltaTime;
        }
    }
    
    private void HandleLookShotStart()
    {
        if (TestInputDownHookShot())
        {
            RaycastHit hit;
            if (Physics.Raycast(myCameraHead.position, myCameraHead.forward, out hit))
            {
                hitPointTransform.position = hit.point;
                hookShotPosition = hit.point;

                hookShotSize = 0f;
                grapplingHook.gameObject.SetActive(true);
                state = State.HookShotThrown;
            }
        }
    }

    private void ThrowHook()
    {
        grapplingHook.LookAt(hookShotPosition);

        float hookShotThrowSpeed = 60f;
        hookShotSize += hookShotThrowSpeed * Time.deltaTime;
        grapplingHook.localScale = new Vector3(1, 1, hookShotSize);

        if (hookShotSize >= Vector3.Distance(transform.position, hookShotPosition))
        {
            state = State.HookShotFlyingPlayer;
            FindObjectOfType<CameraMove>().ZoomIn(120f);
            warpParticles.Play();
        }
    }

    private void HandleHookShotMovement()
    {
        grapplingHook.LookAt(hookShotPosition);

        //calculate the direction in which our player should be moving
        Vector3 hookShotDirection = (hookShotPosition - transform.position).normalized;

        float hookShotMinSpeed = 12f, hookShotMaxSpeed = 50f;

        float hookShotSpeedModifier = Mathf.Clamp(
            Vector3.Distance(transform.position, hookShotPosition),
            hookShotMinSpeed,
            hookShotMaxSpeed);

        myCharacterController.Move(hookShotDirection * hookShotSpeed * hookShotSpeedModifier * Time.deltaTime);

        if (Vector3.Distance(transform.position, hookShotPosition) < 2f)
        {
            //player has gravity again
            StopHookShot();
        }

        //hit the E key again while we are flying
        if(TestInputDownHookShot())
        {
            StopHookShot();
        }

        if(TestInputJump())
        {
            float extraMomentum = 40f, jumpSpeedUp = 70f;
            flyingCharacterMomentum += hookShotDirection * hookShotSpeed * extraMomentum;
            flyingCharacterMomentum += Vector3.up * jumpSpeedUp;
            StopHookShot();
        }
    }

    private bool TestInputJump()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    private bool TestInputDownHookShot()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    private void ResetGraviy()
    {
        velocity.y = 0f;
    }

    private void StopHookShot()
    {
        FindObjectOfType<CameraMove>().ZoomOut();
        grapplingHook.gameObject.SetActive(false);
        state = State.Normal;
        ResetGraviy();
        warpParticles.Stop();
    }
}
