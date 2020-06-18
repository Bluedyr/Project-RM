using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour{

    //OLD



    //The goal of this class is to do the basic movement abiities of the player
    //Generic Movement
    //  Walking                 TWEAK
    //  Running
    //  Sneaking
    //  Jumping                 TWEAK
    //      Mid-air movement    TWEAK
    //  Leaning


    //DONE means I think it is perfect
    //TWEAK means the code is done but parameters need to be changed

    public CharacterController controller;

    [Header("Movement")]
    public float movementSpeed = 5f;//change this later
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;

    //Jumping and Gravity
    //Jumping
    [Header("Jumping & Gravity")]
    public float airSpeed = 0.005f;//speed you can move horizontally in air
    public float jumpHeight = 3f;
    public float gravity = -35f; //gravity -35
    public float groundGravity = -15f;
    [HideInInspector]
    public bool canJump;

    //GROUND CHECKING
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public LayerMask objectMask;
    

    bool isGrounded;

    [HideInInspector]
    public Vector3 velocity;
    float x;
    float z;

    bool a = false;//TODO RENAME THIS
    Vector3 move = new Vector3(0, 0, 0);

    //LEANING
    [Header("Leaning")]
    public float leanSpeed = 100f;
    public float maxLeanAngle = 20f;
    float curLeanAngle = 0;
    bool isLeaning = false;
    bool cancelLean = false;            //USED TO CANCEL A LEAN THAT IS ONGOING (dont set to true when not leaning)
    bool canLean;
    float startLeanRotY;
    [Tooltip("How far the player needs to turn for the lean to cancel")]
    public float leanYRestrict = 30f;

    // Update is called once per frame
    void Update()    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (!isGrounded) {
            //if it isnt on ground, check if it is on an object
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, objectMask);
        }
        if (isGrounded) {
            controller.stepOffset = 0.4f;
            if (Input.GetAxisRaw("Jump") == 0) {
                canJump = true;
            }
            if ((Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E))) {
                canLean = true;
            }
            //sprinting
            if (Input.GetAxisRaw("Sprint") > 0) {
                movementSpeed = sprintSpeed;

            }
            else if (Input.GetAxisRaw("Sprint") == 0) {
                movementSpeed = walkSpeed;

            }
            if (Input.GetAxisRaw("Vertical") <= 0) {
                movementSpeed = walkSpeed;

            }
            //end sprinting
        }
      

        if(isGrounded && velocity.y < 0) {
            velocity.y = groundGravity;//This makes it so we go down slopes smoothly
        }
        
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        if (!isGrounded) {
            if (!a) {
                //make it so move doesnt change after jumping
                move = transform.right * x + transform.forward * z;
                a = true;
            }
            move += transform.right * (x * airSpeed) + transform.forward * (z * airSpeed);
        }
        else {
            if (Input.GetAxisRaw("Lean")!=0&& canLean) {
                move = new Vector3(0, 0, 0);
            }
            else {
                move = transform.right * x + transform.forward * z;
            }
            a = false;
        }

        
        //JUMPING
        if (Input.GetAxisRaw("Jump") >0&& isGrounded && canJump) {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            canJump = false;
            canLean = false;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity*Time.deltaTime);
        controller.Move(move * movementSpeed * Time.deltaTime);

        //LEANING
        float lean = Input.GetAxisRaw("Lean");

        if (lean > 0 && isGrounded && !cancelLean&&canLean) {
            print("left");
            //lean left
            if (isLeaning == false) {
                startLeanRotY = transform.localEulerAngles.y;
            }
            curLeanAngle = Mathf.MoveTowardsAngle(curLeanAngle, maxLeanAngle, leanSpeed * Time.deltaTime);
            isLeaning = true;
            canJump = false;
            transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y, curLeanAngle);
        }
        else if (lean < 0 && isGrounded && !cancelLean && canLean) {
            //lean right
            if (isLeaning == false) {
                startLeanRotY = transform.localEulerAngles.y;
            }
            curLeanAngle = Mathf.MoveTowardsAngle(curLeanAngle, -maxLeanAngle, leanSpeed * Time.deltaTime);
            isLeaning = true;
            canJump = false;

            transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y, curLeanAngle);
        }
        else if (lean == 0 || cancelLean) {
            //reset from lean
            if (curLeanAngle!=0) {
                curLeanAngle = Mathf.MoveTowardsAngle(curLeanAngle, 0f, leanSpeed * Time.deltaTime);
                transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y, curLeanAngle);
                if (curLeanAngle==0) {
                    isLeaning = false;
                    canJump = true;
                }
            }
            //if it doesn't 100% work, remove the if curLeanAngle==0 block above
            //isLeaning = false

        }

        if (Mathf.Abs(transform.localEulerAngles.y-startLeanRotY)>leanYRestrict) {
            //if the player has looked too much to the left or right, cancel the lean
            cancelLean = true;
        }
        if (lean== 0) {
            //If the lean got canceled for looking too much left/right, allow the player to lean again when they stop their input
            cancelLean = false;
        }
       
    }

    private void OnDrawGizmos() {
        Gizmos.DrawSphere(groundCheck.position,groundDistance);
    }

    void lean() {

    }
}
