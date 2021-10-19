using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Title: PlayerController
 * Author: Marc Askew
 */

public class PlayerController : MonoBehaviour {

    #region Variables
    [Header("Movement Variables")]
    public float playerHeight;


    [Header("Movement Variables")]
    public float walkSpeed;
    public float sprintSpeed;
    public float sneakSpeed;
    [HideInInspector] public float movementSpeed;
    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public Vector3 move;

    [Header("Collision")]
    public float pushPower = 1f;


    [Header("Gravity Variables")]
    public bool enableGravity;
    public float gravity = 2.5f;
    bool grounded;
    [HideInInspector]
    public float currentGravity = 0;


    [Header("GroundCheck")]
    public LayerMask excludePlayer;//Everything->Uncheck player
    public CapsuleCollider capsuleCol;//The ground snapping will automatically handle stairs and small objects
    public Transform groundCheckTransform;
    
    public float groundCheckRadius = 0.4f;
    public bool groundSnap;
    public float groundSnapDistance;

    [Header("Jumping")]
    public float jumpHeight;
    [HideInInspector]
    public bool canJump;

    [Header("Leaning Variables")]
    public float leanSpeed = 100f;
    public float maxLeanAngle = 20f;
    float curLeanAngle = 0;
    bool leaning = false;
    //bool canLean;
    //LEAN RESTRICTION
    float startLeanRotY;
    [Tooltip("How far the player needs to turn for the lean to cancel")]
    public float leanYRestrict = 30f;
    bool stopLean;


    //ABILITIES
    AbilityController ac;
   


    #endregion

    void Start() {
        movementSpeed = walkSpeed;
        canJump = true;
        currentGravity = 0f;

        ac = GetComponent<AbilityController>();
        

    }

    
    void Update() {
        
        Gravity();//Adds to gravity force
        Lean();
        Jump();
        
        SimpleMove();
        FinalMove();
        CollisionCheck();
        GroundCheck();//We do this after movement to make snapping smooth

        
    }

    #region Movement Methods
    void SimpleMove() {
        if (grounded) {
            if (leaning) {
                move = new Vector3(0, 0, 0);
            }
            else {
                move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            }

            if (Input.GetAxisRaw("Sprint") == 1) {
                movementSpeed = sprintSpeed;
            }
            else {
                movementSpeed = walkSpeed;
            }
            
        }
        
        
    }

    void FinalMove() {
        
        if (grounded) {
            velocity = new Vector3(move.x * movementSpeed, 0, move.z * movementSpeed);
            velocity = transform.TransformDirection(velocity);//velocity is relative to forward
            
        }
        else {
            //do air movement here
        }
        if (enableGravity) {
            velocity = new Vector3(velocity.x, currentGravity, velocity.z);//Gravity is added here so that it isn't affected by the TransformDirection
        }
        else {
            velocity = new Vector3(velocity.x, velocity.y, velocity.z);//this line isnt needed. just for readability
        }

        transform.position += velocity * Time.deltaTime;
    }
    #endregion

    #region Gravity/Grounding
    void Gravity() {
        if (enableGravity) {
            if (!grounded) {
                currentGravity += gravity * Time.deltaTime;
            }
            else {
                currentGravity = 0;
            }
        }

    }

    void GroundCheck() {
        //if going up dont check
        if (currentGravity<=0&&!ac.isGrappling) {
            
            grounded = Physics.CheckSphere(groundCheckTransform.position, groundCheckRadius, excludePlayer);
           
            if (grounded&&groundSnap) {
                
                //this part is where I do slope snapping
                RaycastHit hit;
                if (Physics.Raycast(transform.position, -transform.up, out hit, groundSnapDistance,excludePlayer)) {
                    
                    Vector3 snapPos = new Vector3(transform.position.x, (hit.point.y + playerHeight / 2), transform.position.z);
                    transform.position = snapPos;
                }
            }
        }
        else {
            grounded = false;
        }
      

        


    }

    #endregion

    #region Collision
    void CollisionCheck() {
        Collider[] overlaps = new Collider[4];//I dont know why only a size of 4
        Collider myCollider = new Collider();
        int num = 0;
        if (capsuleCol != null) {
            num = Physics.OverlapCapsuleNonAlloc(transform.TransformPoint(capsuleCol.center + transform.up * (capsuleCol.height / 2 - capsuleCol.radius)), transform.TransformPoint(capsuleCol.center - transform.up * (capsuleCol.height / 2 - capsuleCol.radius)), capsuleCol.radius,overlaps,excludePlayer, QueryTriggerInteraction.UseGlobal);

            myCollider = capsuleCol;
        }

        for (int i = 0; i < num; i++) {

            Transform t = overlaps[i].transform;
            Vector3 dir;
            float dist;

            if (Physics.ComputePenetration(myCollider, transform.position, transform.rotation, overlaps[i], t.position, t.rotation, out dir, out dist)) {
                Vector3 penetrationVector = dir * dist;
                transform.position = transform.position + penetrationVector;

                //push rigid bodies we come into contact with based on our movement direction as well as direction to it's center of mass
                Rigidbody body = overlaps[i].attachedRigidbody;
                if (body != null && !body.isKinematic) {
                    Vector3 pushDir = new Vector3(velocity.x, 0, velocity.z)+ body.position-transform.position;

                    body.velocity += pushDir * (((pushPower*Time.deltaTime) * movementSpeed) / body.mass);

                    
                    

                }

            }

        }
    }
    #endregion

    #region Jump
    void Jump() {
        if (Input.GetKeyDown(KeyCode.Space)&&grounded&&canJump) {
            float g = Mathf.Sqrt(jumpHeight * -2f * gravity);
            currentGravity = g;
        }
    }
    #endregion


    #region Lean
    void Lean() {
        float lean = Input.GetAxisRaw("Lean");
        if (lean > 0 && grounded&&!stopLean) {
            //lean left
            if (leaning == false) {
                startLeanRotY = transform.localEulerAngles.y;

            }
            curLeanAngle = Mathf.MoveTowardsAngle(curLeanAngle, maxLeanAngle, leanSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y, curLeanAngle);

            leaning = true;
            canJump = false;
        }
        else if (lean < 0 && grounded && !stopLean) {
            //lean right
            if (leaning == false) {
                startLeanRotY = transform.localEulerAngles.y;
            }
            curLeanAngle = Mathf.MoveTowardsAngle(curLeanAngle, -maxLeanAngle, leanSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y, curLeanAngle);

            leaning = true;
            canJump = false;
        }
        else if (lean == 0||!grounded||stopLean) {
            //reset from lean
            if (curLeanAngle != 0) {
                curLeanAngle = Mathf.MoveTowardsAngle(curLeanAngle, 0f, leanSpeed * Time.deltaTime);
                transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y, curLeanAngle);
                if (curLeanAngle == 0) {
                    leaning = false;
                    canJump = true;
                    
                }
            }
            //if it doesn't 100% work, remove the if curLeanAngle==0 block above
            //isLeaning = false

        }

        if (leaning) {
            if (!(Mathf.Abs(transform.localEulerAngles.y - startLeanRotY) < leanYRestrict)) {
                if (!(Mathf.Abs(transform.localEulerAngles.y - startLeanRotY) > 360 - leanYRestrict)) {
                    stopLean = true;
                }
            }

        }
        if (lean == 0) {
            //If the lean got canceled for looking too much left/right, allow the player to lean again when they stop their input
            stopLean = false;
        }

    }

    #endregion

    private void OnDrawGizmos() {

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheckTransform.position, groundCheckRadius);//ground check
        
        Gizmos.DrawRay(transform.position, -transform.up * groundSnapDistance);
    }
}


